namespace Chat_Client
{
    partial class Form1
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
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportChatHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chatView = new System.Windows.Forms.RichTextBox();
            this.inputMessageTextbox = new System.Windows.Forms.TextBox();
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.connectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileToolStripMenuItem, this.aboutToolStripMenuItem });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(529, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.exportChatHistoryToolStripMenuItem });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportChatHistoryToolStripMenuItem
            // 
            this.exportChatHistoryToolStripMenuItem.Name = "exportChatHistoryToolStripMenuItem";
            this.exportChatHistoryToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.exportChatHistoryToolStripMenuItem.Text = "Export chat history...";
            this.exportChatHistoryToolStripMenuItem.Click += new System.EventHandler(this.exportChatHistoryToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.infoToolStripMenuItem });
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.infoToolStripMenuItem.Text = "Info...";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // chatView
            // 
            this.chatView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.chatView.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.chatView.Location = new System.Drawing.Point(5, 4);
            this.chatView.Name = "chatView";
            this.chatView.ReadOnly = true;
            this.chatView.Size = new System.Drawing.Size(495, 290);
            this.chatView.TabIndex = 1;
            this.chatView.Text = "";
            // 
            // inputMessageTextbox
            // 
            this.inputMessageTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.inputMessageTextbox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.inputMessageTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputMessageTextbox.Location = new System.Drawing.Point(44, 302);
            this.inputMessageTextbox.Name = "inputMessageTextbox";
            this.inputMessageTextbox.Size = new System.Drawing.Size(362, 28);
            this.inputMessageTextbox.TabIndex = 2;
            this.inputMessageTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputMessageTextbox_KeyDown);
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendMessageButton.Location = new System.Drawing.Point(412, 299);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(88, 33);
            this.sendMessageButton.TabIndex = 3;
            this.sendMessageButton.Text = "Send";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.sendMessageButton_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openFileButton.BackColor = System.Drawing.Color.Transparent;
            this.openFileButton.BackgroundImage = global::Chat_Client.Properties.Resources.attachment_icon;
            this.openFileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.openFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openFileButton.Location = new System.Drawing.Point(5, 299);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(33, 33);
            this.openFileButton.TabIndex = 5;
            this.openFileButton.UseVisualStyleBackColor = false;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // connectButton
            // 
            this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connectButton.Location = new System.Drawing.Point(373, 41);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(134, 38);
            this.connectButton.TabIndex = 6;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 41);
            this.label1.TabIndex = 7;
            this.label1.Text = "Chat with Server";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 439);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(529, 22);
            this.statusBar1.TabIndex = 8;
            this.statusBar1.Text = "Status OK";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.openFileButton);
            this.panel1.Controls.Add(this.sendMessageButton);
            this.panel1.Controls.Add(this.inputMessageTextbox);
            this.panel1.Controls.Add(this.chatView);
            this.panel1.Location = new System.Drawing.Point(7, 81);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 341);
            this.panel1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Chat Client";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.StatusBar statusBar1;
        private System.Windows.Forms.Button sendMessageButton;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Button openFileButton;

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportChatHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.RichTextBox chatView;
        private System.Windows.Forms.TextBox inputMessageTextbox;
        private System.Windows.Forms.Button connectButton;

        #endregion
    }
}