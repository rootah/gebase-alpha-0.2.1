using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gebase_alpha_0._2._1
{
    internal class paycode
    {
        public static string connectionString;
        public static MongoClient client;
        public static MongoServer server;
        public static MongoDatabase gebase;
        public static MongoCollection<stdcolls> stdcollection;

        public static void MongoInitiate(MainAppForm mainapp)
        {
            connectionString = Properties.Settings.Default.ServerOne;
            client = new MongoClient(connectionString);
            server = client.GetServer();
            gebase = server.GetDatabase("gebase");
            stdcollection = gebase.GetCollection<stdcolls>("stds");

            PayGridRefresh(mainapp);
        }

        public static void PayGridRefresh(MainAppForm mainapp)
        {
            var payresult = new BindingList<stdcolls>(stdcollection.FindAll().ToList());
            mainapp.gridPayments.DataSource = payresult;
            PayColHide(mainapp);
        }

        public static void PayColHide(MainAppForm mainapp)
        {
            var i = 0;
            while (i <= 16)
            {
                mainapp.bandedPaymentsGridView.Columns[i].VisibleIndex = -1;
                i++;
            }

            var j = 19;

            while (j <= 22)
            {
                mainapp.bandedPaymentsGridView.Columns[j].VisibleIndex = -1;
                j++;
            }

            mainapp.bandedPaymentsGridView.Columns["group"].Width = 5;
        }
    }
}
