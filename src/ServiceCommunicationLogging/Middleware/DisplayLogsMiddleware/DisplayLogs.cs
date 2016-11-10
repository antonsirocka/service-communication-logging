using LogsDataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommunicationLogging.Middleware.DisplayLogsMiddleware
{
    public class DisplayLogs : Microsoft.AspNetCore.DiagnosticsViewPage.Views.BaseView
    {
        private readonly ILogPersister _logPersister;

        public DisplayLogs(DisplayLogsModel displayLogsModel, ILogPersister logPersister)
        {
            this.DisplayLogsModel = displayLogsModel;
            this._logPersister = logPersister;
        }

        public DisplayLogsModel DisplayLogsModel { get; set; }

        public List<LogsDataAccess.ServiceMessageLog> ServiceMessageLogs { get; set; }

        public override async Task ExecuteAsync()
        {
            var date = Request.Query["date"];
            var context = Request.Query["context"];

            string lookupValue = string.Empty;

            if (date.Count == 0 && context.Count == 0)
            {
                lookupValue = DateTime.Now.ToString("yyyyMMdd");
            }

            this.ServiceMessageLogs = await this._logPersister.Get(lookupValue);

            Response.ContentType = "text/html; charset=utf-8";

            WriteLiteral(@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"" />
                    <title>ASP.NET Core Logs</title>
                    <script src=""//ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.1.min.js""></script>
                    <style>
                        body {
                                font-size: .813em;
                                white-space: nowrap;
                                margin: 20px;
                             }
                        form { 
                                display: inline-block;
                            }
                        h1 {
                            margin-left: 25px;
                        }
                        table {
                            margin: 0px auto;
                            border-collapse: collapse;
                            border-spacing: 0px;
                            table-layout: fixed;
                            width: 100%;
                        }
                        td, th {
                            padding: 4px;
                        }
                        thead {
                            font-size: 1em;
                            font-family: Arial;
                        }
                        tr {
                            height: 23px;
                        }
                        #requestHeader {
                            border-bottom: solid 1px gray;
                            border-top: solid 1px gray;
                            margin-bottom: 2px;
                            font-size: 1em;
                            line-height: 2em;
                        }
                        #searchBar {
                            margin-bottom: 20px;
                        }
                        .collapse {
                            color: black;
                            float: right;
                            font-weight: normal;
                            width: 1em;
                        }
                        .date, .time {
                            width: 70px;
                        }
                        .logHeader {
                            border-bottom: 1px solid lightgray;
                            color: gray;
                            text-align: left;
                        }
                        .logState {
                            text-overflow: ellipsis;
                            overflow: hidden;
                        }
                        .logTd {
                            border-left: 1px solid gray;
                            padding: 0px;
                        }
                        .logs {
                            width: 80%;
                        }
                        .logRow:hover {
                            background-color: #D6F5FF;
                        }
                        .requestRow>td {
                            border-bottom: solid 1px gray;
                        }
                        body {
                            font-family: 'Segoe UI', Tahoma, Arial, Helvtica, sans-serif;
                            line-height: 1.4em;
                        }
                        h1 {
                            font-family: 'Segoe UI', Helvetica, sans-serif;
                            font-size: 2.5em;
                        }
                    </style>
                </head>
                <body>
                    
                    <div id=""searchBar"">
                        <select id=""searchBy"">
                            <option value=""date"">Date</option>
                            <option value=""context"">Context ID</option>
                        </select>
                        <input type=""textbox"" id=""txtSearch"" />
                        <input type=""button"" id=""btnSearch"" value=""search"" />
                    </div>
                    
                    <table id=""requestTable"">
                        <thead id=""requestHeader"">
                            <tr>
                                <th>Serviced by</th>
                                <th>Request date</th>
                                <th>Response date</th>
                                <th>Execution time</th>    
                                <th>Trace</th>
                                <th>Request</th>
                                <th>Response</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class=""requestRow"">
                                
                            </tr>
                        </tbody>
                   </table>    

                    <script type=""text/javascript"">
                        $(function () {
                            $('#searchBy').on('change', function () {
                                SetDefaultSearchValue($(this));
                            });

                            $('#btnSearch').on('click', function () {
                                
                            });

                            SetDefaultSearchValue($('#searchBy'));
                        });

                        function SetDefaultSearchValue(searchSelector) {
                            if (searchSelector.val() == 'date') {
                                $('#txtSearch').val(new Date().formatDDMMYYYY());
                            }
                            else if (searchSelector.val() == 'context') {
                                $('#txtSearch').val('');
                            }
                        }

                        Date.prototype.formatDDMMYYYY = function() {
                            return this.getDate() + 
                            '/' +  (this.getMonth() + 1) +
                            '/' + this.getFullYear();
                        }
                    </script>
                    
                </body>
                </html>");
        }
    }
}
