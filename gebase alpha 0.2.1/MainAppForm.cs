using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace gebase_alpha_0._2._1
{
    public partial class MainAppForm : RibbonForm
    {
        public schedulercoll _schedentity;

        public MainAppForm()
        {
            InitializeComponent();
        }

        private void backstageViewButtonItem1_ItemClick(object sender, BackstageViewItemEventArgs e)
        {
            Application.Exit();
        }

        private void ribbonControl_SelectedPageChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RibbonTabIndex = ribbonControl.SelectedPage.PageIndex;
            Properties.Settings.Default.Save();
            RibbonTabRefresh();

            xtraTabControl1.SelectedTabPageIndex = ribbonControl.SelectedPage.PageIndex;
        }

        private void RibbonTabRefresh()
        {
            ribbonControl.SelectedPage = ribbonControl.Pages[Properties.Settings.Default.RibbonTabIndex];

            switch (Properties.Settings.Default.RibbonTabIndex)
            {
                case 0:
                    groupcode.MongoInitiate();
                    groupcode.GroupTabShow(this);
                    groupcode.GroupGridColHide(this);
                    bandedGroupGridView.FocusedRowHandle = 0;
                    bandedGroupGridView_FocusedRowChanged(null, null);
                    break;
                case 1:
                    stdcode.MongoInitiate(this);
                    stdcode.StdTabShow(this);
                    stdcode.StdGridColHide(this);
                    bandedStudentsGridView.FocusedRowHandle = 0;
                    bandedStudentsGridView_FocusedRowChanged(null, null);
                    break;
                case 2:
                    SchedGrid.GoToToday();
                    break;
                case 3:
                    paycode.MongoInitiate(this);
                    break;
            }
        }

        private void MainAppForm_Load(object sender, EventArgs e)
        {
            RibbonTabRefresh();
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
        }

        private void ActiveGroupButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFilterFlag = "active";
            Properties.Settings.Default.Save();
            groupcode.GroupGridRefresh(this);

            bandedGroupGridView_FocusedRowChanged(null, null);
        }

        private void AwaitingGroupButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFilterFlag = "awaiting";
            Properties.Settings.Default.Save();
            groupcode.GroupGridRefresh(this);

            bandedGroupGridView_FocusedRowChanged(null, null);
        }

        private void PausedGroupButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFilterFlag = "paused";
            Properties.Settings.Default.Save();
            groupcode.GroupGridRefresh(this);
            bandedGroupGridView_FocusedRowChanged(null, null);
        }

        private void FinishedGroupButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFilterFlag = "finished";
            Properties.Settings.Default.Save();
            groupcode.GroupGridRefresh(this);
            bandedGroupGridView_FocusedRowChanged(null, null);
        }

        private void PauseGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "_id").ToString();
            var number = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();
            groupcode.GroupAction(this, _id, "paused", number);
        }

        private void ResumeGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "_id").ToString();
            var number = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();
            groupcode.GroupAction(this, _id, "active", number);
        }

        private void StartGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "_id").ToString();
            var number = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();
            groupcode.GroupAction(this, _id, "active", number);
        }

        private void FinishGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "_id").ToString();
            var number = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();
            groupcode.GroupAction(this, _id, "finished", number);
        }

        private void DeleteGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "_id").ToString();
            var number = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();

            if (MessageBox.Show(String.Format("Group {0} will be deleted", number), "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                groupcode.GroupDel(this, _id);
            }
        }

        private void AddGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFormType = "add";
            var groupadd = new groupform(this) { StartPosition = FormStartPosition.CenterParent };
            groupadd.simpleButtonOk.Visible = true;
            groupadd.simpleButtonEdit.Visible = false;
            groupadd.ShowDialog();
        }

        private void EditGroupButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.GroupFormType = "edit";
            var groupadd = new groupform(this) { StartPosition = FormStartPosition.CenterParent };
            groupadd.simpleButtonOk.Visible = true;
            groupadd.simpleButtonEdit.Visible = false;
            groupadd.ShowDialog();
        }

        private void AddStudentsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFormType = "add";
            var stdadd = new studentform(this);
            stdadd.StartPosition = FormStartPosition.CenterParent;
            stdadd.Text = stdadd.Text + " [new]";
            stdadd.ShowDialog();
        }

        private void DeleteStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MessageBox.Show("Really delete?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var _id = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "_id").ToString();
                stdcode.StdRemove(this, _id);
            }
        }

        private void PauseStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "_id").ToString();
            stdcode.StdActionButton(this, _id, "paused");
        }

        private void ResumeStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "_id").ToString();
            stdcode.StdActionButton(this, _id, "active");
        }

        private void StartStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "_id").ToString();
            stdcode.StdActionButton(this, _id, "active");
        }

        private void FinishStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _id = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "_id").ToString();
            stdcode.StdActionButton(this, _id, "finished");
        }

        private void ActiveStudentsButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFilterFlag = "active";
            Properties.Settings.Default.Save();

            stdcode.StdGridRefresh(this);
            bandedStudentsGridView_FocusedRowChanged(null, null);
        }

        private void PausedStudentsButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFilterFlag = "paused";
            Properties.Settings.Default.Save();

            stdcode.StdGridRefresh(this);
            bandedStudentsGridView_FocusedRowChanged(null, null);
        }

        private void AwaitingStudentsButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFilterFlag = "awaiting";
            Properties.Settings.Default.Save();

            stdcode.StdGridRefresh(this);
            bandedStudentsGridView_FocusedRowChanged(null, null);
        }

        private void FinishedStudentsButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFilterFlag = "finished";
            Properties.Settings.Default.Save();

            stdcode.StdGridRefresh(this);
            bandedStudentsGridView_FocusedRowChanged(null, null);
        }

        private void EditStudentButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Properties.Settings.Default.StdFormType = "edit";
            var stdadd = new studentform(this);
            stdadd.StartPosition = FormStartPosition.CenterParent;
            stdadd.Text = stdadd.Text + " [edit]";
            stdadd.ShowDialog();
        }

        private void bandedStudentsGridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                var status = bandedStudentsGridView.GetRowCellValue(bandedStudentsGridView.FocusedRowHandle, "status").ToString();
                stdcode.StdActionButtonsSwitch(this, status);
            }
            catch
            {
                PauseStudentButton.Enabled = false;
                ResumeStudentButton.Enabled = false;
                FinishStudentButton.Enabled = false;
                StartStudentButton.Enabled = false;
                DeleteStudentButton.Enabled = false;
                EditStudentButton.Enabled = false;
            }
        }

        private void bandedGroupGridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.CurrentGroupNumber = bandedGroupGridView.GetRowCellValue(bandedGroupGridView.FocusedRowHandle, "number").ToString();
                Properties.Settings.Default.Save();

                groupcode.ActionButtonSwitch(this);
            }
            catch
            {
                Properties.Settings.Default.CurrentGroupNumber = string.Empty;
                Properties.Settings.Default.Save();

                PauseGroupButton.Enabled = false;
                ResumeGroupButton.Enabled = false;
                FinishGroupButton.Enabled = false;
                StartGroupButton.Enabled = false;
                DeleteGroupButton.Enabled = false;
                EditGroupButton.Enabled = false;
            }

            stdcode.GroupDetails(this);
        }

        public void DetailGroupButton_DownChanged(object sender, ItemClickEventArgs e)
        {
            if (DetailGroupButton.Down)
            {
                Properties.Settings.Default.GroupDetailsShow = true;
                Properties.Settings.Default.Save();
                splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;

                DetailGroupGridViewColHide();
            }
            else
            {
                Properties.Settings.Default.GroupDetailsShow = false;
                Properties.Settings.Default.Save();
                splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            }
        }

        public void DetailGroupGridViewColHide()
        {
            try
            {
                var i = 0;
                while (i <= 17)
                {
                    bandedDetailGroupGridView.Columns[i].VisibleIndex = -1;
                    i++;
                }
                var j = 19;
                while (j <= bandedDetailGroupGridView.Columns.Count)
                {
                    bandedDetailGroupGridView.Columns[j].VisibleIndex = -1;
                    j++;
                }
            }
            catch
            {
                return;
            }
        }
    }
}
