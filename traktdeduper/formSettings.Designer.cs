namespace traktdeduper
{
    partial class formSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formSettings));
            this.label_clientSecret = new System.Windows.Forms.Label();
            this.label_username = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_accessToken = new System.Windows.Forms.Label();
            this.textBox_accessToken = new System.Windows.Forms.TextBox();
            this.textBox_clientSecret = new System.Windows.Forms.TextBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_clientID = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label_clientSecret
            // 
            this.label_clientSecret.AutoSize = true;
            this.label_clientSecret.Location = new System.Drawing.Point(11, 73);
            this.label_clientSecret.Name = "label_clientSecret";
            this.label_clientSecret.Size = new System.Drawing.Size(47, 13);
            this.label_clientSecret.TabIndex = 0;
            this.label_clientSecret.Text = "Client ID";
            // 
            // label_username
            // 
            this.label_username.AutoSize = true;
            this.label_username.Location = new System.Drawing.Point(11, 26);
            this.label_username.Name = "label_username";
            this.label_username.Size = new System.Drawing.Size(86, 13);
            this.label_username.TabIndex = 2;
            this.label_username.Text = "Trakt Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Client Secret";
            // 
            // label_accessToken
            // 
            this.label_accessToken.AutoSize = true;
            this.label_accessToken.Location = new System.Drawing.Point(11, 180);
            this.label_accessToken.Name = "label_accessToken";
            this.label_accessToken.Size = new System.Drawing.Size(76, 13);
            this.label_accessToken.TabIndex = 6;
            this.label_accessToken.Text = "Access Token";
            // 
            // textBox_accessToken
            // 
            this.textBox_accessToken.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::traktdeduper.Properties.Settings.Default, "AccessToken", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox_accessToken.Location = new System.Drawing.Point(12, 197);
            this.textBox_accessToken.Name = "textBox_accessToken";
            this.textBox_accessToken.ReadOnly = true;
            this.textBox_accessToken.Size = new System.Drawing.Size(237, 20);
            this.textBox_accessToken.TabIndex = 7;
            this.textBox_accessToken.Text = global::traktdeduper.Properties.Settings.Default.AccessToken;
            // 
            // textBox_clientSecret
            // 
            this.textBox_clientSecret.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::traktdeduper.Properties.Settings.Default, "clientSecret", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox_clientSecret.Location = new System.Drawing.Point(12, 142);
            this.textBox_clientSecret.Name = "textBox_clientSecret";
            this.textBox_clientSecret.Size = new System.Drawing.Size(237, 20);
            this.textBox_clientSecret.TabIndex = 5;
            this.textBox_clientSecret.Text = global::traktdeduper.Properties.Settings.Default.clientSecret;
            // 
            // textBox_username
            // 
            this.textBox_username.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::traktdeduper.Properties.Settings.Default, "username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox_username.Location = new System.Drawing.Point(12, 43);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(237, 20);
            this.textBox_username.TabIndex = 3;
            this.textBox_username.Text = global::traktdeduper.Properties.Settings.Default.username;
            // 
            // textBox_clientID
            // 
            this.textBox_clientID.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::traktdeduper.Properties.Settings.Default, "clientID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox_clientID.Location = new System.Drawing.Point(12, 90);
            this.textBox_clientID.Name = "textBox_clientID";
            this.textBox_clientID.Size = new System.Drawing.Size(237, 20);
            this.textBox_clientID.TabIndex = 1;
            this.textBox_clientID.Text = global::traktdeduper.Properties.Settings.Default.clientID;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(271, 43);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(244, 174);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // formSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox_accessToken);
            this.Controls.Add(this.label_accessToken);
            this.Controls.Add(this.textBox_clientSecret);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_username);
            this.Controls.Add(this.label_username);
            this.Controls.Add(this.textBox_clientID);
            this.Controls.Add(this.label_clientSecret);
            this.Name = "formSettings";
            this.Text = "Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formSettings_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_clientSecret;
        private System.Windows.Forms.TextBox textBox_clientID;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.Label label_username;
        private System.Windows.Forms.TextBox textBox_clientSecret;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_accessToken;
        private System.Windows.Forms.Label label_accessToken;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}