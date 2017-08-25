using ARSoft.Tools.Net.Dns;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VtnrNetRadioServer.Helper;

namespace VtnrNetRadioServer.DnsServer2
{
    public class ForwardingDnsServer
    {
        private readonly VtunerConfig _cfg;
        private readonly ILogger<ForwardingDnsServer> _logger;
        private readonly NetworkInterfaceHelper _networkInterfaceHelper;

        public ForwardingDnsServer(
            IOptions<VtunerConfig> cfg,
            ILogger<ForwardingDnsServer> logger,
            NetworkInterfaceHelper networkInterfaceHelper)
        {
            this._cfg = cfg.Value;
            this._logger = logger;
            this._networkInterfaceHelper = networkInterfaceHelper;
        }

        public DnsServer Run()
        {
            var server = new DnsServer(System.Net.IPAddress.Any, 1, 1);
            server.QueryReceived += OnQueryReceived;
            server.Start();
            return server;
        }

        private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage message = e.Query as DnsMessage;
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;

            if (message.Questions.Count != 1)
            {
                return;
            }

            var question = query.Questions[0];
            _logger.LogInformation("dns-req: {0} {1}", question.RecordType, question.Name);

            DnsMessage response = query.CreateResponseInstance();
            
            if (question.RecordType == RecordType.A
                //&& question.Name.IsSubDomainOf(ARSoft.Tools.Net.DomainName.Parse("vtuner.com"))
                && (question.Name.ToString() == _cfg.VtunerServerOne 
                    || question.Name.ToString() == _cfg.VtunerServerTwo))
            {
                var myIp = _networkInterfaceHelper.GetMyIPv4Address();
                response.AnswerRecords.Add(new ARecord(question.Name, 10, myIp));

                response.ReturnCode = ReturnCode.NoError;

                // set the response
                e.Response = response;
                return;
            }

            // send query to upstream server
            DnsMessage upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);

            // if got an answer, copy it to the message sent to the client
            if (upstreamResponse != null)
            {
                foreach (DnsRecordBase record in (upstreamResponse.AnswerRecords))
                {
                    response.AnswerRecords.Add(record);
                }
                foreach (DnsRecordBase record in (upstreamResponse.AdditionalRecords))
                {
                    response.AdditionalRecords.Add(record);
                }

                response.ReturnCode = ReturnCode.NoError;

                // set the response
                e.Response = response;
            }

        }
    }
}
