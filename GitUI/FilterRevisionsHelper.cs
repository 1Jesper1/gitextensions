using System;
using System.Windows.Forms;
using GitCommands;

namespace GitUI
{
    public class FilterRevisionsHelper : IDisposable
    {
        private ToolStripTextBox _NO_TRANSLATE_textBox;
        private ToolStripDropDownButton _NO_TRANSLATE_dropDownButton;
        private RevisionGrid _NO_TRANSLATE_revisionGrid;
        private ToolStripLabel _NO_TRANSLATE_label;
        private ToolStripButton _NO_TRANSLATE_showFirstParentButton;

        private ToolStripMenuItem _commitToolStripMenuItem;
        private ToolStripMenuItem _committerToolStripMenuItem;
        private ToolStripMenuItem _authorToolStripMenuItem;
        private ToolStripMenuItem _diffContainsToolStripMenuItem;
        private ToolStripMenuItem _hashToolStripMenuItem;

        private Form _NO_TRANSLATE_form;

        public FilterRevisionsHelper()
        {
            _commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _committerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _authorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _diffContainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _hashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            //
            // commitToolStripMenuItem1
            //
            _commitToolStripMenuItem.Checked = true;
            _commitToolStripMenuItem.CheckOnClick = true;
            _commitToolStripMenuItem.Name = "commitToolStripMenuItem1";
            _commitToolStripMenuItem.Text = "Commit";
            //
            // committerToolStripMenuItem
            //
            _committerToolStripMenuItem.CheckOnClick = true;
            _committerToolStripMenuItem.Name = "committerToolStripMenuItem";
            _committerToolStripMenuItem.Text = "Committer";
            //
            // authorToolStripMenuItem
            //
            _authorToolStripMenuItem.CheckOnClick = true;
            _authorToolStripMenuItem.Name = "authorToolStripMenuItem";
            _authorToolStripMenuItem.Text = "Author";
            //
            // diffContainsToolStripMenuItem
            //
            _diffContainsToolStripMenuItem.CheckOnClick = true;
            _diffContainsToolStripMenuItem.Name = "diffContainsToolStripMenuItem";
            _diffContainsToolStripMenuItem.Text = "Diff contains (SLOW)";
            _diffContainsToolStripMenuItem.Click += diffContainsToolStripMenuItem_Click;
            //
            // hashToolStripMenuItem
            //
            _hashToolStripMenuItem.CheckOnClick = true;
            _hashToolStripMenuItem.Name = "hashToolStripMenuItem";
            _hashToolStripMenuItem.Size = new System.Drawing.Size(216, 24);
            _hashToolStripMenuItem.Text = "Hash";
        }

        public FilterRevisionsHelper(ToolStripTextBox textBox, ToolStripDropDownButton dropDownButton, RevisionGrid revisionGrid, ToolStripLabel label, ToolStripButton showFirstParentButton, Form form)
            : this()
        {
            _NO_TRANSLATE_dropDownButton = dropDownButton;
            _NO_TRANSLATE_textBox = textBox;
            _NO_TRANSLATE_revisionGrid = revisionGrid;
            _NO_TRANSLATE_label = label;
            _NO_TRANSLATE_showFirstParentButton = showFirstParentButton;
            _NO_TRANSLATE_form = form;

            _NO_TRANSLATE_dropDownButton.DropDownItems.AddRange(new ToolStripItem[]
            {
                _commitToolStripMenuItem,
                _committerToolStripMenuItem,
                _authorToolStripMenuItem,
                _diffContainsToolStripMenuItem
            });

            _NO_TRANSLATE_showFirstParentButton.Checked = AppSettings.ShowFirstParent;

            _NO_TRANSLATE_label.Click += ToolStripLabelClick;
            _NO_TRANSLATE_textBox.Leave += ToolStripTextBoxFilterLeave;
            _NO_TRANSLATE_textBox.KeyPress += ToolStripTextBoxFilterKeyPress;
            _NO_TRANSLATE_showFirstParentButton.Click += ToolStripShowFirstParentButtonClick;
            _NO_TRANSLATE_revisionGrid.ShowFirstParentsToggled += RevisionGridShowFirstParentsToggled;
        }

        public void SetFilter(string filter)
        {
            _NO_TRANSLATE_textBox.Text = filter;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string revListArgs;
            string inMemMessageFilter;
            string inMemCommitterFilter;
            string inMemAuthorFilter;
            var filterParams = new bool[4];
            filterParams[0] = _commitToolStripMenuItem.Checked;
            filterParams[1] = _committerToolStripMenuItem.Checked;
            filterParams[2] = _authorToolStripMenuItem.Checked;
            filterParams[3] = _diffContainsToolStripMenuItem.Checked;
            try
            {
                _NO_TRANSLATE_revisionGrid.FormatQuickFilter(_NO_TRANSLATE_textBox.Text,
                                               filterParams,
                                               out revListArgs,
                                               out inMemMessageFilter,
                                               out inMemCommitterFilter,
                                               out inMemAuthorFilter);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(_NO_TRANSLATE_form, ex.Message, "Filter error");
                _NO_TRANSLATE_textBox.Text = "";
                return;
            }

            if ((_NO_TRANSLATE_revisionGrid.QuickRevisionFilter == revListArgs) &&
                (_NO_TRANSLATE_revisionGrid.InMemMessageFilter == inMemMessageFilter) &&
                (_NO_TRANSLATE_revisionGrid.InMemCommitterFilter == inMemCommitterFilter) &&
                (_NO_TRANSLATE_revisionGrid.InMemAuthorFilter == inMemAuthorFilter) &&
                (_NO_TRANSLATE_revisionGrid.InMemFilterIgnoreCase))
            {
                return;
            }

            _NO_TRANSLATE_revisionGrid.QuickRevisionFilter = revListArgs;
            _NO_TRANSLATE_revisionGrid.InMemMessageFilter = inMemMessageFilter;
            _NO_TRANSLATE_revisionGrid.InMemCommitterFilter = inMemCommitterFilter;
            _NO_TRANSLATE_revisionGrid.InMemAuthorFilter = inMemAuthorFilter;
            _NO_TRANSLATE_revisionGrid.InMemFilterIgnoreCase = true;
            _NO_TRANSLATE_revisionGrid.Visible = true;
            _NO_TRANSLATE_revisionGrid.ForceRefreshRevisions();
        }

        private void ToolStripTextBoxFilterLeave(object sender, EventArgs e)
        {
            ToolStripLabelClick(sender, e);
        }

        private void ToolStripTextBoxFilterKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ToolStripLabelClick(null, null);
            }
        }

        private void ToolStripLabelClick(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ToolStripShowFirstParentButtonClick(object sender, EventArgs e)
        {
            _NO_TRANSLATE_revisionGrid.ShowFirstParent_ToolStripMenuItemClick(sender, e);
        }

        private void RevisionGridShowFirstParentsToggled(object sender, EventArgs e)
        {
            _NO_TRANSLATE_showFirstParentButton.Checked = AppSettings.ShowFirstParent;
        }

        private void diffContainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_diffContainsToolStripMenuItem.Checked)
            {
                _commitToolStripMenuItem.Checked = false;
                _committerToolStripMenuItem.Checked = false;
                _authorToolStripMenuItem.Checked = false;
                _hashToolStripMenuItem.Checked = false;
            }
            else
            {
                _commitToolStripMenuItem.Checked = true;
            }
        }

        public void SetLimit(int limit)
        {
            _NO_TRANSLATE_revisionGrid.SetLimit(limit);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commitToolStripMenuItem.Dispose();
                _committerToolStripMenuItem.Dispose();
                _authorToolStripMenuItem.Dispose();
                _diffContainsToolStripMenuItem.Dispose();
                _hashToolStripMenuItem.Dispose();
            }
        }
    }
}