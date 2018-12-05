namespace reversi
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label labelRedScoreHeader;
            System.Windows.Forms.TableLayoutPanel tableLayout;
            System.Windows.Forms.TableLayoutPanel tableLayoutControls;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.labelBlueScoreHeader = new System.Windows.Forms.Label();
            this.labelScoreBlue = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonNewGame = new System.Windows.Forms.Button();
            this.labelScoreRed = new System.Windows.Forms.Label();
            this.checkHelp = new System.Windows.Forms.CheckBox();
            this.humanPlayer = new reversi.BoardControl();
            labelRedScoreHeader = new System.Windows.Forms.Label();
            tableLayout = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutControls = new System.Windows.Forms.TableLayoutPanel();
            tableLayout.SuspendLayout();
            tableLayoutControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRedScoreHeader
            // 
            labelRedScoreHeader.AutoSize = true;
            labelRedScoreHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelRedScoreHeader.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelRedScoreHeader.ForeColor = System.Drawing.Color.Red;
            labelRedScoreHeader.Location = new System.Drawing.Point(4, 0);
            labelRedScoreHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelRedScoreHeader.Name = "labelRedScoreHeader";
            labelRedScoreHeader.Size = new System.Drawing.Size(139, 74);
            labelRedScoreHeader.TabIndex = 0;
            labelRedScoreHeader.Text = "Red:";
            labelRedScoreHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayout
            // 
            tableLayout.ColumnCount = 1;
            tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayout.Controls.Add(tableLayoutControls, 0, 0);
            tableLayout.Controls.Add(this.humanPlayer, 0, 1);
            tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayout.Location = new System.Drawing.Point(0, 0);
            tableLayout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            tableLayout.Name = "tableLayout";
            tableLayout.RowCount = 2;
            tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 185F));
            tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayout.Size = new System.Drawing.Size(568, 709);
            tableLayout.TabIndex = 1;
            // 
            // tableLayoutControls
            // 
            tableLayoutControls.ColumnCount = 5;
            tableLayoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            tableLayoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 267F));
            tableLayoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 147F));
            tableLayoutControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutControls.Controls.Add(this.labelBlueScoreHeader, 3, 0);
            tableLayoutControls.Controls.Add(this.labelScoreBlue, 3, 1);
            tableLayoutControls.Controls.Add(this.labelStatus, 2, 0);
            tableLayoutControls.Controls.Add(this.buttonNewGame, 2, 1);
            tableLayoutControls.Controls.Add(labelRedScoreHeader, 1, 0);
            tableLayoutControls.Controls.Add(this.labelScoreRed, 1, 1);
            tableLayoutControls.Controls.Add(this.checkHelp, 2, 2);
            tableLayoutControls.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutControls.Location = new System.Drawing.Point(4, 4);
            tableLayoutControls.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            tableLayoutControls.Name = "tableLayoutControls";
            tableLayoutControls.RowCount = 3;
            tableLayoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            tableLayoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutControls.Size = new System.Drawing.Size(560, 177);
            tableLayoutControls.TabIndex = 1;
            // 
            // labelBlueScoreHeader
            // 
            this.labelBlueScoreHeader.AutoSize = true;
            this.labelBlueScoreHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelBlueScoreHeader.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBlueScoreHeader.ForeColor = System.Drawing.Color.Blue;
            this.labelBlueScoreHeader.Location = new System.Drawing.Point(418, 0);
            this.labelBlueScoreHeader.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBlueScoreHeader.Name = "labelBlueScoreHeader";
            this.labelBlueScoreHeader.Size = new System.Drawing.Size(139, 74);
            this.labelBlueScoreHeader.TabIndex = 0;
            this.labelBlueScoreHeader.Text = "Blue:";
            this.labelBlueScoreHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelScoreBlue
            // 
            this.labelScoreBlue.AutoSize = true;
            this.labelScoreBlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelScoreBlue.Font = new System.Drawing.Font("Impact", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScoreBlue.ForeColor = System.Drawing.Color.Blue;
            this.labelScoreBlue.Location = new System.Drawing.Point(418, 74);
            this.labelScoreBlue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelScoreBlue.Name = "labelScoreBlue";
            tableLayoutControls.SetRowSpan(this.labelScoreBlue, 2);
            this.labelScoreBlue.Size = new System.Drawing.Size(139, 103);
            this.labelScoreBlue.TabIndex = 1;
            this.labelScoreBlue.Text = "2";
            this.labelScoreBlue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(151, 6);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(4, 6, 4, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(259, 68);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Red is on move";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonNewGame
            // 
            this.buttonNewGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonNewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewGame.Location = new System.Drawing.Point(151, 78);
            this.buttonNewGame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNewGame.Name = "buttonNewGame";
            this.buttonNewGame.Size = new System.Drawing.Size(259, 43);
            this.buttonNewGame.TabIndex = 1;
            this.buttonNewGame.Text = "New game";
            this.buttonNewGame.UseVisualStyleBackColor = true;
            this.buttonNewGame.Click += new System.EventHandler(this.buttonNewGame_Click);
            // 
            // labelScoreRed
            // 
            this.labelScoreRed.AutoSize = true;
            this.labelScoreRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelScoreRed.Font = new System.Drawing.Font("Impact", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScoreRed.ForeColor = System.Drawing.Color.Red;
            this.labelScoreRed.Location = new System.Drawing.Point(4, 74);
            this.labelScoreRed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelScoreRed.Name = "labelScoreRed";
            tableLayoutControls.SetRowSpan(this.labelScoreRed, 2);
            this.labelScoreRed.Size = new System.Drawing.Size(139, 103);
            this.labelScoreRed.TabIndex = 1;
            this.labelScoreRed.Text = "2";
            this.labelScoreRed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkHelp
            // 
            this.checkHelp.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkHelp.AutoSize = true;
            this.checkHelp.Checked = true;
            this.checkHelp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.checkHelp.Location = new System.Drawing.Point(151, 129);
            this.checkHelp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkHelp.Name = "checkHelp";
            this.checkHelp.Size = new System.Drawing.Size(259, 44);
            this.checkHelp.TabIndex = 2;
            this.checkHelp.Text = "Help";
            this.checkHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkHelp.UseVisualStyleBackColor = true;
            this.checkHelp.CheckedChanged += new System.EventHandler(this.checkHelp_CheckedChanged);
            // 
            // board
            // 
            this.humanPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.humanPlayer.Cursor = System.Windows.Forms.Cursors.Default;
            this.humanPlayer.Location = new System.Drawing.Point(4, 189);
            this.humanPlayer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.humanPlayer.Name = "board";
            this.humanPlayer.ShowHints = true;
            this.humanPlayer.Size = new System.Drawing.Size(560, 516);
            this.humanPlayer.TabIndex = 2;
            this.humanPlayer.UpdateStatus += new reversi.BoardControl.UpdateStatusEventHandler(this.board_UpdateStatus);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 709);
            this.Controls.Add(tableLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = this.Size;
            this.Name = "MainWindow";
            this.Text = "Reversi";
            tableLayout.ResumeLayout(false);
            tableLayoutControls.ResumeLayout(false);
            tableLayoutControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BoardControl humanPlayer;
        private System.Windows.Forms.Label labelScoreRed;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonNewGame;
        private System.Windows.Forms.Label labelScoreBlue;
        private System.Windows.Forms.Label labelBlueScoreHeader;
        private System.Windows.Forms.CheckBox checkHelp;
    }
}

