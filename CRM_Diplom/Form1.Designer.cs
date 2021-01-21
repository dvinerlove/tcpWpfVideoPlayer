
namespace CRM_Diplom
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.stringChat = new System.Windows.Forms.RichTextBox();
            this.stringMessage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.stringPort = new System.Windows.Forms.TextBox();
            this.stringHost = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.recievedMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // stringChat
            // 
            this.stringChat.BackColor = System.Drawing.SystemColors.Window;
            this.stringChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stringChat.Location = new System.Drawing.Point(707, 156);
            this.stringChat.Name = "stringChat";
            this.stringChat.ReadOnly = true;
            this.stringChat.Size = new System.Drawing.Size(294, 64);
            this.stringChat.TabIndex = 15;
            this.stringChat.Text = "";
            // 
            // stringMessage
            // 
            this.stringMessage.Location = new System.Drawing.Point(316, 296);
            this.stringMessage.Name = "stringMessage";
            this.stringMessage.Size = new System.Drawing.Size(139, 20);
            this.stringMessage.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(704, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "log";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(924, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(837, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "ip";
            // 
            // stringPort
            // 
            this.stringPort.Location = new System.Drawing.Point(858, 61);
            this.stringPort.Name = "stringPort";
            this.stringPort.Size = new System.Drawing.Size(60, 20);
            this.stringPort.TabIndex = 9;
            this.stringPort.Text = "6565";
            // 
            // stringHost
            // 
            this.stringHost.Location = new System.Drawing.Point(655, 62);
            this.stringHost.Name = "stringHost";
            this.stringHost.Size = new System.Drawing.Size(166, 20);
            this.stringHost.TabIndex = 10;
            this.stringHost.Text = "25.109.150.192";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(956, 57);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 16;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(461, 296);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(66, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "Send";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(316, 62);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(139, 212);
            this.listBox1.TabIndex = 17;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(461, 62);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(96, 20);
            this.textBox1.TabIndex = 18;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(461, 88);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(96, 24);
            this.button3.TabIndex = 21;
            this.button3.Text = "login";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // recievedMessage
            // 
            this.recievedMessage.Location = new System.Drawing.Point(707, 254);
            this.recievedMessage.Name = "recievedMessage";
            this.recievedMessage.Size = new System.Drawing.Size(294, 20);
            this.recievedMessage.TabIndex = 22;
            this.recievedMessage.TextChanged += new System.EventHandler(this.recievedMessage_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 523);
            this.Controls.Add(this.recievedMessage);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.stringChat);
            this.Controls.Add(this.stringMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stringPort);
            this.Controls.Add(this.stringHost);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox stringChat;
        private System.Windows.Forms.TextBox stringMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox stringPort;
        private System.Windows.Forms.TextBox stringHost;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox recievedMessage;
    }
}

