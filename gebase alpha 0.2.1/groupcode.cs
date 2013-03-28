using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraBars;
using MongoDB.Bson;
using DevExpress.Utils;

namespace gebase_alpha_0._2._1
{
    public class groupcode
    {
        public static string connectionString;
        public static MongoClient client;
        public static MongoServer server;
        public static MongoDatabase gebase;
        public static MongoCollection<groupcolls> groupcollection;

        public groupcolls _groupentity;

        public static void GroupGridColHide(MainAppForm mainapp)
        {
            mainapp.bandedGroupGridView.Columns["_id"].Visible = false;
            mainapp.bandedGroupGridView.Columns["suntime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["montime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["tuetime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["wedtime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["thutime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["fritime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["sattime"].Visible = false;
            mainapp.bandedGroupGridView.Columns["start"].Visible = false;
            mainapp.bandedGroupGridView.Columns["hours"].Visible = false;
            mainapp.bandedGroupGridView.Columns["kind"].Visible = false;
            mainapp.bandedGroupGridView.Columns["status"].Visible = false;
            mainapp.bandedGroupGridView.Columns["stdcount"].Width = 5;
            mainapp.bandedGroupGridView.Columns["stdcount"].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
        }

        public static void MongoInitiate()
        {
            connectionString = Properties.Settings.Default.ServerOne;
            client = new MongoClient(connectionString);
            server = client.GetServer();
            gebase = server.GetDatabase("gebase");
            groupcollection = gebase.GetCollection<groupcolls>("groups");
        }

        public static void ActionButtonSwitch(MainAppForm mainapp)
        {
            var groupstatus = Properties.Settings.Default.GroupFilterFlag;

            switch (groupstatus)
            {
                case "active":
                    mainapp.PauseGroupButton.Enabled = true;
                    mainapp.ResumeGroupButton.Enabled = false;
                    mainapp.FinishGroupButton.Enabled = true;
                    mainapp.StartGroupButton.Enabled = false;
                    mainapp.EditGroupButton.Enabled = true;
                    mainapp.DeleteGroupButton.Enabled = true;
                    break;
                case "paused":
                    mainapp.PauseGroupButton.Enabled = false;
                    mainapp.ResumeGroupButton.Enabled = true;
                    mainapp.FinishGroupButton.Enabled = true;
                    mainapp.StartGroupButton.Enabled = false;
                    mainapp.EditGroupButton.Enabled = true;
                    mainapp.DeleteGroupButton.Enabled = true;
                    break;
                case "awaiting":
                    mainapp.PauseGroupButton.Enabled = false;
                    mainapp.ResumeGroupButton.Enabled = false;
                    mainapp.FinishGroupButton.Enabled = false;
                    mainapp.StartGroupButton.Enabled = true;
                    mainapp.EditGroupButton.Enabled = true;
                    mainapp.DeleteGroupButton.Enabled = true;
                    break;
                case "finished":
                    mainapp.PauseGroupButton.Enabled = false;
                    mainapp.ResumeGroupButton.Enabled = false;
                    mainapp.FinishGroupButton.Enabled = false;
                    mainapp.StartGroupButton.Enabled = false;
                    mainapp.EditGroupButton.Enabled = true;
                    mainapp.DeleteGroupButton.Enabled = true;
                    break;
            }
        }

        public static void GroupAction(MainAppForm mainapp, string _id, string groupstatus, string number)
        {
            gebase.GetCollection<groupcolls>("groups").Update(
                Query.EQ("_id", ObjectId.Parse(_id)),
                MongoDB.Driver.Builders.Update.Set("status", groupstatus));

            GroupGridRefresh(mainapp);

            mainapp.StatusEventsText.Caption = String.Format("Group {0} state changed to {1}", number, groupstatus.ToUpper());
            mainapp.StatusEventsText.Visibility = BarItemVisibility.Always;
        }

        public static void GroupStdCount(MainAppForm mainapp)
        {
            stdcode.MongoInitiate(mainapp);

            var query =
                                      from e in gebase.GetCollection<groupcolls>("groups").AsQueryable<groupcolls>()
                                      select e.number;

            foreach (var number in query)
            {
                var stdcount = Convert.ToInt16(stdcode.gebase.GetCollection<stdcolls>("stds").FindAs<stdcolls>(Query.EQ("group", number)).Count());

                gebase.GetCollection<groupcolls>("groups").Update(
                Query.EQ("number", number),
                MongoDB.Driver.Builders.Update.Set("stdcount", stdcount));
            }
        }

        public static void GroupGridRefresh(MainAppForm mainapp)
        {
            var query = Query.EQ("status", Properties.Settings.Default.GroupFilterFlag);

            var groupresult = new BindingList<groupcolls>(gebase.GetCollection<groupcolls>("groups").Find(query).ToList());
            mainapp.gridGroup.DataSource = groupresult;

            var count = Convert.ToInt16(groupresult.Count());
            mainapp.ItemsCountStatusText.Caption = String.Format("{0} groups count: {1}", Convert.ToString(Properties.Settings.Default.GroupFilterFlag), Convert.ToString(count));
            mainapp.bandedGroupGridView.GroupPanelText = Convert.ToString(Properties.Settings.Default.GroupFilterFlag) + " groups list";
        }

        public static void GroupDel(MainAppForm mainapp, string _id)
        {
            gebase.GetCollection<groupcolls>("groups").Remove(
                Query.EQ("_id", ObjectId.Parse(_id)));

            groupcode.GroupGridRefresh(mainapp);

            mainapp.bandedGroupGridView.FocusedRowHandle = Properties.Settings.Default.GroupSelectedRowIndex;
        }

        public static void GroupTabShow(MainAppForm mainapp)
        {
            switch (Properties.Settings.Default.GroupFilterFlag)
            {
                case "active":
                    mainapp.ActiveGroupButton.Down = true;
                    break;
                case "paused":
                    mainapp.PausedGroupButton.Down = true;
                    break;
                case "awaiting":
                    mainapp.AwaitingGroupButton.Down = true;
                    break;
                case "finished":
                    mainapp.FinishedGroupButton.Down = true;
                    break;
            }

            switch (Properties.Settings.Default.GroupDetailsShow)
            {
                case true:
                    mainapp.DetailGroupButton.Down = true;
                    break;
                case false:
                    mainapp.DetailGroupButton.Down = false;
                    break;
            }

            mainapp.DetailGroupButton_DownChanged(null, null);
            GroupStdCount(mainapp);
            GroupGridRefresh(mainapp);
        }

        public static void GroupDetailView(MainAppForm mainapp)
        {
            switch (Properties.Settings.Default.GroupDetailsShow)
            {
                case true:
                    mainapp.DetailGroupButton.Down = true;
                    mainapp.DetailGroupButton_DownChanged(null, null);
                    break;
                case false:
                    mainapp.DetailGroupButton.Down = false;
                    mainapp.DetailGroupButton_DownChanged(null, null);
                    break;
            }
        }

        public static void GetCurrentGroupData(MainAppForm mainapp)
        {
        }
    }
}
