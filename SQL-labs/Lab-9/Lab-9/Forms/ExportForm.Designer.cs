namespace AgroDbApp.Forms
{
    partial class ExportForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            checkBoxOperationJournal = new System.Windows.Forms.CheckBox();
            checkBoxSowingSummary = new System.Windows.Forms.CheckBox();
            checkBoxTechniqueInfo = new System.Windows.Forms.CheckBox();
            groupBoxFormat = new System.Windows.Forms.GroupBox();
            radioButtonHtml = new System.Windows.Forms.RadioButton();
            radioButtonExcel = new System.Windows.Forms.RadioButton();
            buttonExport = new System.Windows.Forms.Button();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            groupBoxFormat.SuspendLayout();
            SuspendLayout();
            // 
            // checkBoxOperationJournal
            // 
            checkBoxOperationJournal.AutoSize = true;
            checkBoxOperationJournal.Location = new System.Drawing.Point(30, 30);
            checkBoxOperationJournal.Name = "checkBoxOperationJournal";
            checkBoxOperationJournal.Size = new System.Drawing.Size(152, 19);
            checkBoxOperationJournal.TabIndex = 0;
            checkBoxOperationJournal.Text = "Журнал операций";
            checkBoxOperationJournal.UseVisualStyleBackColor = true;
            // 
            // checkBoxSowingSummary
            // 
            checkBoxSowingSummary.AutoSize = true;
            checkBoxSowingSummary.Location = new System.Drawing.Point(30, 65);
            checkBoxSowingSummary.Name = "checkBoxSowingSummary";
            checkBoxSowingSummary.Size = new System.Drawing.Size(145, 19);
            checkBoxSowingSummary.TabIndex = 1;
            checkBoxSowingSummary.Text = "Сводка по посеву";
            checkBoxSowingSummary.UseVisualStyleBackColor = true;
            // 
            // checkBoxTechniqueInfo
            // 
            checkBoxTechniqueInfo.AutoSize = true;
            checkBoxTechniqueInfo.Location = new System.Drawing.Point(30, 100);
            checkBoxTechniqueInfo.Name = "checkBoxTechniqueInfo";
            checkBoxTechniqueInfo.Size = new System.Drawing.Size(163, 19);
            checkBoxTechniqueInfo.TabIndex = 2;
            checkBoxTechniqueInfo.Text = "Информация по технике";
            checkBoxTechniqueInfo.UseVisualStyleBackColor = true;
            // 
            // groupBoxFormat
            // 
            groupBoxFormat.Controls.Add(radioButtonHtml);
            groupBoxFormat.Controls.Add(radioButtonExcel);
            groupBoxFormat.Location = new System.Drawing.Point(30, 140);
            groupBoxFormat.Name = "groupBoxFormat";
            groupBoxFormat.Size = new System.Drawing.Size(200, 90);
            groupBoxFormat.TabIndex = 3;
            groupBoxFormat.TabStop = false;
            groupBoxFormat.Text = "Формат экспорта";
            // 
            // radioButtonHtml
            // 
            radioButtonHtml.AutoSize = true;
            radioButtonHtml.Location = new System.Drawing.Point(15, 55);
            radioButtonHtml.Name = "radioButtonHtml";
            radioButtonHtml.Size = new System.Drawing.Size(118, 19);
            radioButtonHtml.TabIndex = 1;
            radioButtonHtml.TabStop = true;
            radioButtonHtml.Text = "Экспорт в HTML";
            radioButtonHtml.UseVisualStyleBackColor = true;
            // 
            // radioButtonExcel
            // 
            radioButtonExcel.AutoSize = true;
            radioButtonExcel.Location = new System.Drawing.Point(15, 25);
            radioButtonExcel.Name = "radioButtonExcel";
            radioButtonExcel.Size = new System.Drawing.Size(112, 19);
            radioButtonExcel.TabIndex = 0;
            radioButtonExcel.TabStop = true;
            radioButtonExcel.Text = "Экспорт в Excel";
            radioButtonExcel.UseVisualStyleBackColor = true;
            // 
            // buttonExport
            // 
            buttonExport.Location = new System.Drawing.Point(260, 170);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new System.Drawing.Size(110, 35);
            buttonExport.TabIndex = 4;
            buttonExport.Text = "Выполнить";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // ExportForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(400, 260);
            Controls.Add(buttonExport);
            Controls.Add(groupBoxFormat);
            Controls.Add(checkBoxTechniqueInfo);
            Controls.Add(checkBoxSowingSummary);
            Controls.Add(checkBoxOperationJournal);
            Name = "ExportForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Экспорт отчетных данных";
            groupBoxFormat.ResumeLayout(false);
            groupBoxFormat.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.CheckBox checkBoxOperationJournal;
        private System.Windows.Forms.CheckBox checkBoxSowingSummary;
        private System.Windows.Forms.CheckBox checkBoxTechniqueInfo;
        private System.Windows.Forms.GroupBox groupBoxFormat;
        private System.Windows.Forms.RadioButton radioButtonHtml;
        private System.Windows.Forms.RadioButton radioButtonExcel;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}