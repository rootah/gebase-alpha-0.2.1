using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace gebase_alpha_0._2._1
{
    public partial class groupform : DevExpress.XtraEditors.XtraForm
    {
        public int state = 0;
        private MainAppForm mainapp;

        public const string connectionString = "mongodb://localhost";
        public static MongoClient client = new MongoClient(connectionString);
        public static MongoServer server = client.GetServer();
        public static MongoDatabase gebase = server.GetDatabase("gebase");
        private MongoCollection<groupcolls> groupcollection = gebase.GetCollection<groupcolls>("groups");
        public groupcolls _groupentity;

        public groupform(MainAppForm mainform)
        {
            InitializeComponent();
            mainapp = mainform;
        }

        private void groupform_Load(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.GroupFormType)
            {
                case "add":
                    Text = "group [add]";
                    simpleButtonEdit.Visible = false;
                    simpleButtonOk.Visible = true;
                    NewGroupNumber();
                    break;
                case "edit":
                    if (mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "time").ToString() == "... custom")
                    {
                        textEditGtime.EditValue = null;
                        Height = 361;
                        state = 1;
                    }
                    else
                    {
                        textEditGtime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "time").ToString());
                        Height = 224;
                        state = 0;
                    }

                    Text = "group [edit : " + Convert.ToString(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "number").ToString()) + "]";
                    simpleButtonEdit.Visible = true;
                    simpleButtonOk.Visible = false;

                    textEditGnum.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "number").ToString();
                    textEditGteacher.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "teacher").ToString();
                    textEditGlevel.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "level").ToString();
                    textEditGdays.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "days").ToString();
                    checkedhours.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "hours").ToString();
                    textEditGstatus.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "status").ToString();
                    textEditGstart.Text = mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "start").ToString();

                    textEditGdays_Properties_EditValueChanged(null, null);
                    CustomTimeGet();
                    break;
            }
        }

        private void CustomTimeButton_Click(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.GroupFormType)
            {
                case "edit":
                    switch (state)
                    {
                        case 0:
                            textEditGtime.EditValue = null;

                            Height = 361;
                            state = 1;
                            CustomTimeGet();
                            break;

                        case 1:
                            Height = 224;
                            state = 0;
                            if (mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "time").ToString() != "... custom")
                            {
                                textEditGtime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "time").ToString());
                            }
                            else
                            {
                                textEditGtime.EditValue = null;
                            }
                            break;
                    }
                    break;
                case "add":
                    switch (state)
                    {
                        case 0:
                            textEditGtime.EditValue = null;

                            Height = 361;
                            state = 1;
                            break;

                        case 1:
                            Height = 224;
                            state = 0;
                            break;
                    }
                    break;
            }
        }

        private void simpleButtonEdit_Click(object sender, EventArgs e)
        {
            string gtime;
            string groupkind;

            if (textEditGtime.EditValue == null)
            {
                gtime = "... custom";
                groupkind = "customtime";
            }
            else
            {
                gtime = textEditGtime.Time.ToShortTimeString();
                groupkind = "sametime";
            }

            gebase.GetCollection<groupcolls>("groups").Update(
                Query.EQ("_id", ObjectId.Parse(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "_id").ToString())),
                MongoDB.Driver.Builders.Update
                .Set("teacher", textEditGteacher.Text)
                .Set("number", textEditGnum.Text)
                .Set("level", textEditGlevel.Text)
                .Set("days", textEditGdays.Text)
                .Set("hours", Convert.ToInt16(checkedhours.Text))
                .Set("start", textEditGstart.DateTime.ToShortDateString())
                .Set("status", textEditGstatus.Text)
                .Set("time", gtime)
                .Set("kind", groupkind)

                .Set("suntime", suntime.Time.ToShortTimeString())
                .Set("montime", montime.Time.ToShortTimeString())
                .Set("tuetime", tuetime.Time.ToShortTimeString())
                .Set("wedtime", wedtime.Time.ToShortTimeString())
                .Set("thutime", thutime.Time.ToShortTimeString())
                .Set("fritime", fritime.Time.ToShortTimeString())
                .Set("sattime", sattime.Time.ToShortTimeString())
            );

            groupcode.GroupGridRefresh(mainapp);
            mainapp.bandedGroupGridView.FocusedRowHandle = Properties.Settings.Default.GroupSelectedRowIndex;
            mainapp.StatusEventsText.Caption = "Group " + textEditGnum.Text.ToString() + " edited";
            mainapp.StatusEventsText.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            Close();
        }

        private void NewGroupNumber()
        {
            string oldfullnum;
            string newfullnum;
            int firsthalfoldnum;
            int firsthalfnewnum;
            string lasthalfoldnum;
            int index;

            Query.Exists("number");
            var notnull = Convert.ToInt16(groupcollection.FindAll().Count());
            var sortDescending = SortBy.Descending("number");

            if (notnull > 0)
            {
                var maxresult = groupcollection.FindAll()
                         .SetFields(Fields.Include("number"))
                         .SetSortOrder(sortDescending).SetLimit(1).First();

                oldfullnum = maxresult.number;
                index = oldfullnum.IndexOf("-");
                firsthalfoldnum = Convert.ToInt16(oldfullnum.Substring(0, index));
                firsthalfnewnum = firsthalfoldnum + 1;
                lasthalfoldnum = oldfullnum.Substring(index + 1, 2);
                newfullnum = Convert.ToString(firsthalfnewnum) + "-" + lasthalfoldnum;

                textEditGnum.Text = newfullnum;
            }
            else
            {
                textEditGnum.Text = string.Empty;
            }
        }

        private void textEditGdays_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (textEditGdays.Properties.Items["mon"].CheckState == CheckState.Checked)
            {
                monlabel.Enabled = true;
                montime.Enabled = true;
            }
            else
            {
                monlabel.Enabled = false;
                montime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["tue"].CheckState == CheckState.Checked)
            {
                tuelabel.Enabled = true;
                tuetime.Enabled = true;
            }
            else
            {
                tuelabel.Enabled = false;
                tuetime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["wed"].CheckState == CheckState.Checked)
            {
                wedlabel.Enabled = true;
                wedtime.Enabled = true;
            }
            else
            {
                wedlabel.Enabled = false;
                wedtime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["thu"].CheckState == CheckState.Checked)
            {
                thulabel.Enabled = true;
                thutime.Enabled = true;
            }
            else
            {
                thulabel.Enabled = false;
                thutime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["fri"].CheckState == CheckState.Checked)
            {
                frilabel.Enabled = true;
                fritime.Enabled = true;
            }
            else
            {
                frilabel.Enabled = false;
                fritime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["sat"].CheckState == CheckState.Checked)
            {
                satlabel.Enabled = true;
                sattime.Enabled = true;
            }
            else
            {
                satlabel.Enabled = false;
                sattime.Enabled = false;
            }

            if (textEditGdays.Properties.Items["sun"].CheckState == CheckState.Checked)
            {
                sunlabel.Enabled = true;
                suntime.Enabled = true;
            }
            else
            {
                sunlabel.Enabled = false;
                suntime.Enabled = false;
            }
        }

        private void simpleButtonOk_Click(object sender, EventArgs e)
        {
            string gtime;
            string groupkind;

            if (textEditGtime.EditValue == null)
            {
                gtime = "... custom";
                groupkind = "customtime";
            }
            else
            {
                gtime = textEditGtime.Time.ToShortTimeString();
                groupkind = "sametime";
            }
            var count = Convert.ToInt16(groupcollection.FindAs<groupcolls>(Query.EQ("number", textEditGnum.Text)).Count());
            if (count > 0)
            {
                MessageBox.Show("Same group number exist!");
                return;
            }
            else
            {
                _groupentity = new groupcolls();

                _groupentity = new groupcolls();
                _groupentity.teacher = textEditGteacher.Text;
                _groupentity.number = textEditGnum.Text;
                _groupentity.level = textEditGlevel.Text;
                _groupentity.days = textEditGdays.Text;
                _groupentity.hours = Convert.ToInt16(checkedhours.Text);
                _groupentity.start = textEditGstart.DateTime.ToShortDateString();
                _groupentity.status = textEditGstatus.Text;
                _groupentity.time = gtime;
                _groupentity.kind = groupkind;
                _groupentity.suntime = suntime.Time.ToShortTimeString();
                _groupentity.montime = montime.Time.ToShortTimeString();
                _groupentity.tuetime = tuetime.Time.ToShortTimeString();
                _groupentity.wedtime = wedtime.Time.ToShortTimeString();
                _groupentity.thutime = thutime.Time.ToShortTimeString();
                _groupentity.fritime = fritime.Time.ToShortTimeString();
                _groupentity.sattime = sattime.Time.ToShortTimeString();

                groupcollection.Insert(_groupentity);
                groupcode.GroupGridRefresh(mainapp);
                mainapp.bandedGroupGridView.FocusedRowHandle = Properties.Settings.Default.GroupSelectedRowIndex;
                mainapp.StatusEventsText.Caption = "Group " + textEditGnum.Text.ToString() + " created";
                mainapp.StatusEventsText.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                Close();
            }
        }

        private void textEditGtime_EditValueChanged(object sender, EventArgs e)
        {
            if (suntime.Enabled)
            {
                suntime.Time = textEditGtime.Time;
            }
            if (montime.Enabled)
            {
                montime.Time = textEditGtime.Time;
            }
            if (tuetime.Enabled)
            {
                tuetime.Time = textEditGtime.Time;
            }
            if (wedtime.Enabled)
            {
                wedtime.Time = textEditGtime.Time;
            }
            if (thutime.Enabled)
            {
                thutime.Time = textEditGtime.Time;
            }
            if (fritime.Enabled)
            {
                fritime.Time = textEditGtime.Time;
            }
            if (sattime.Enabled)
            {
                sattime.Time = textEditGtime.Time;
            }
        }

        private void CustomTimeGet()
        {
            if (suntime.Enabled)
            {
                suntime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "suntime").ToString());
            }
            if (montime.Enabled)
            {
                montime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "montime").ToString());
            }
            if (tuetime.Enabled)
            {
                tuetime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "tuetime").ToString());
            }
            if (wedtime.Enabled)
            {
                wedtime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "wedtime").ToString());
            }
            if (thutime.Enabled)
            {
                thutime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "thutime").ToString());
            }
            if (fritime.Enabled)
            {
                fritime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "fritime").ToString());
            }
            if (sattime.Enabled)
            {
                sattime.Time = Convert.ToDateTime(mainapp.bandedGroupGridView.GetRowCellValue(mainapp.bandedGroupGridView.FocusedRowHandle, "sattime").ToString());
            }
        }
    }
}
