namespace AuditionSyntheticPartnerManager;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private Panel shellPanel;

    private Panel headerPanel;

    private Label titleLabel;

    private Label subtitleLabel;

    private Label pathCaptionLabel;

    private Label pathValueLabel;

    private TableLayoutPanel contentLayout;

    private Panel listPanel;

    private Label listTitleLabel;

    private ListBox partnerListBox;

    private FlowLayoutPanel listButtonsPanel;

    private Button addButton;

    private Button cloneButton;

    private Button deleteButton;

    private Panel editorPanel;

    private TableLayoutPanel editorLayout;

    private Label nicknameLabel;

    private TextBox nicknameTextBox;

    private Label userIdLabel;

    private TextBox userIdTextBox;

    private Label genderLabel;

    private NumericUpDown genderNumericUpDown;

    private Label experienceLabel;

    private NumericUpDown experienceNumericUpDown;

    private Label powerLabel;

    private NumericUpDown powerNumericUpDown;

    private Label teamLabel;

    private NumericUpDown teamNumericUpDown;

    private Label readyLabel;

    private CheckBox readyCheckBox;

    private Panel previewPanel;

    private Label previewTitleLabel;

    private Label previewValueLabel;

    private FlowLayoutPanel actionPanel;

    private Button applyButton;

    private Button saveCatalogButton;

    private Button openFolderButton;

    private Label saveStatusLabel;

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
        components = new System.ComponentModel.Container();
        shellPanel = new Panel();
        contentLayout = new TableLayoutPanel();
        listPanel = new Panel();
        partnerListBox = new ListBox();
        listButtonsPanel = new FlowLayoutPanel();
        addButton = new Button();
        cloneButton = new Button();
        deleteButton = new Button();
        listTitleLabel = new Label();
        editorPanel = new Panel();
        editorLayout = new TableLayoutPanel();
        nicknameLabel = new Label();
        nicknameTextBox = new TextBox();
        userIdLabel = new Label();
        userIdTextBox = new TextBox();
        genderLabel = new Label();
        genderNumericUpDown = new NumericUpDown();
        experienceLabel = new Label();
        experienceNumericUpDown = new NumericUpDown();
        powerLabel = new Label();
        powerNumericUpDown = new NumericUpDown();
        teamLabel = new Label();
        teamNumericUpDown = new NumericUpDown();
        readyLabel = new Label();
        readyCheckBox = new CheckBox();
        previewPanel = new Panel();
        previewTitleLabel = new Label();
        previewValueLabel = new Label();
        actionPanel = new FlowLayoutPanel();
        applyButton = new Button();
        saveCatalogButton = new Button();
        openFolderButton = new Button();
        saveStatusLabel = new Label();
        headerPanel = new Panel();
        titleLabel = new Label();
        subtitleLabel = new Label();
        pathCaptionLabel = new Label();
        pathValueLabel = new Label();
        shellPanel.SuspendLayout();
        contentLayout.SuspendLayout();
        listPanel.SuspendLayout();
        listButtonsPanel.SuspendLayout();
        editorPanel.SuspendLayout();
        editorLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)genderNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)experienceNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)powerNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)teamNumericUpDown).BeginInit();
        previewPanel.SuspendLayout();
        actionPanel.SuspendLayout();
        headerPanel.SuspendLayout();
        SuspendLayout();
        shellPanel.BackColor = Color.FromArgb(246, 241, 232);
        shellPanel.Controls.Add(contentLayout);
        shellPanel.Controls.Add(headerPanel);
        shellPanel.Dock = DockStyle.Fill;
        shellPanel.Padding = new Padding(18);
        contentLayout.ColumnCount = 2;
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        contentLayout.Controls.Add(listPanel, 0, 0);
        contentLayout.Controls.Add(editorPanel, 1, 0);
        contentLayout.Dock = DockStyle.Fill;
        contentLayout.Location = new Point(18, 114);
        contentLayout.Margin = new Padding(0);
        contentLayout.RowCount = 1;
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        listPanel.BackColor = Color.FromArgb(35, 52, 68);
        listPanel.Controls.Add(partnerListBox);
        listPanel.Controls.Add(listButtonsPanel);
        listPanel.Controls.Add(listTitleLabel);
        listPanel.Dock = DockStyle.Fill;
        listPanel.Margin = new Padding(0, 0, 18, 0);
        listPanel.Padding = new Padding(18);
        partnerListBox.BackColor = Color.FromArgb(244, 247, 248);
        partnerListBox.BorderStyle = BorderStyle.None;
        partnerListBox.Dock = DockStyle.Fill;
        partnerListBox.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        partnerListBox.FormattingEnabled = true;
        partnerListBox.IntegralHeight = false;
        partnerListBox.ItemHeight = 17;
        partnerListBox.Location = new Point(18, 56);
        partnerListBox.Margin = new Padding(0, 8, 0, 8);
        partnerListBox.SelectedIndexChanged += partnerListBox_SelectedIndexChanged;
        listButtonsPanel.Controls.Add(addButton);
        listButtonsPanel.Controls.Add(cloneButton);
        listButtonsPanel.Controls.Add(deleteButton);
        listButtonsPanel.Dock = DockStyle.Bottom;
        listButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
        listButtonsPanel.Location = new Point(18, 530);
        listButtonsPanel.Margin = new Padding(0);
        listButtonsPanel.Padding = new Padding(0, 12, 0, 0);
        listButtonsPanel.Size = new Size(244, 42);
        addButton.BackColor = Color.FromArgb(242, 188, 90);
        addButton.FlatAppearance.BorderSize = 0;
        addButton.FlatStyle = FlatStyle.Flat;
        addButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        addButton.Location = new Point(0, 12);
        addButton.Margin = new Padding(0, 0, 8, 0);
        addButton.Size = new Size(72, 30);
        addButton.Text = "Nuevo";
        addButton.UseVisualStyleBackColor = false;
        addButton.Click += addButton_Click;
        cloneButton.BackColor = Color.FromArgb(120, 180, 156);
        cloneButton.FlatAppearance.BorderSize = 0;
        cloneButton.FlatStyle = FlatStyle.Flat;
        cloneButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        cloneButton.Location = new Point(80, 12);
        cloneButton.Margin = new Padding(0, 0, 8, 0);
        cloneButton.Size = new Size(72, 30);
        cloneButton.Text = "Clonar";
        cloneButton.UseVisualStyleBackColor = false;
        cloneButton.Click += cloneButton_Click;
        deleteButton.BackColor = Color.FromArgb(194, 87, 74);
        deleteButton.FlatAppearance.BorderSize = 0;
        deleteButton.FlatStyle = FlatStyle.Flat;
        deleteButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        deleteButton.ForeColor = Color.White;
        deleteButton.Location = new Point(160, 12);
        deleteButton.Margin = new Padding(0);
        deleteButton.Size = new Size(72, 30);
        deleteButton.Text = "Quitar";
        deleteButton.UseVisualStyleBackColor = false;
        deleteButton.Click += deleteButton_Click;
        listTitleLabel.AutoSize = true;
        listTitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        listTitleLabel.ForeColor = Color.White;
        listTitleLabel.Location = new Point(18, 18);
        listTitleLabel.Text = "Partners";
        editorPanel.BackColor = Color.White;
        editorPanel.Controls.Add(editorLayout);
        editorPanel.Dock = DockStyle.Fill;
        editorPanel.Margin = new Padding(0);
        editorPanel.Padding = new Padding(24);
        editorLayout.ColumnCount = 2;
        editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        editorLayout.Controls.Add(nicknameLabel, 0, 0);
        editorLayout.Controls.Add(nicknameTextBox, 1, 0);
        editorLayout.Controls.Add(userIdLabel, 0, 1);
        editorLayout.Controls.Add(userIdTextBox, 1, 1);
        editorLayout.Controls.Add(genderLabel, 0, 2);
        editorLayout.Controls.Add(genderNumericUpDown, 1, 2);
        editorLayout.Controls.Add(experienceLabel, 0, 3);
        editorLayout.Controls.Add(experienceNumericUpDown, 1, 3);
        editorLayout.Controls.Add(powerLabel, 0, 4);
        editorLayout.Controls.Add(powerNumericUpDown, 1, 4);
        editorLayout.Controls.Add(teamLabel, 0, 5);
        editorLayout.Controls.Add(teamNumericUpDown, 1, 5);
        editorLayout.Controls.Add(readyLabel, 0, 6);
        editorLayout.Controls.Add(readyCheckBox, 1, 6);
        editorLayout.Controls.Add(previewPanel, 0, 7);
        editorLayout.Controls.Add(actionPanel, 0, 8);
        editorLayout.Controls.Add(saveStatusLabel, 0, 9);
        editorLayout.Dock = DockStyle.Fill;
        editorLayout.RowCount = 10;
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        editorLayout.SetColumnSpan(previewPanel, 2);
        editorLayout.SetColumnSpan(actionPanel, 2);
        editorLayout.SetColumnSpan(saveStatusLabel, 2);
        nicknameLabel.Anchor = AnchorStyles.Left;
        nicknameLabel.AutoSize = true;
        nicknameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        nicknameLabel.Text = "Nickname";
        nicknameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        nicknameTextBox.BorderStyle = BorderStyle.FixedSingle;
        nicknameTextBox.Font = new Font("Segoe UI", 10F);
        nicknameTextBox.TextChanged += editor_ValueChanged;
        userIdLabel.Anchor = AnchorStyles.Left;
        userIdLabel.AutoSize = true;
        userIdLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        userIdLabel.Text = "User ID";
        userIdTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        userIdTextBox.BorderStyle = BorderStyle.FixedSingle;
        userIdTextBox.Font = new Font("Segoe UI", 10F);
        userIdTextBox.TextChanged += editor_ValueChanged;
        genderLabel.Anchor = AnchorStyles.Left;
        genderLabel.AutoSize = true;
        genderLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        genderLabel.Text = "Genero";
        genderNumericUpDown.Anchor = AnchorStyles.Left;
        genderNumericUpDown.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
        genderNumericUpDown.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
        genderNumericUpDown.Size = new Size(120, 25);
        genderNumericUpDown.ValueChanged += editor_ValueChanged;
        experienceLabel.Anchor = AnchorStyles.Left;
        experienceLabel.AutoSize = true;
        experienceLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        experienceLabel.Text = "Experiencia";
        experienceNumericUpDown.Anchor = AnchorStyles.Left;
        experienceNumericUpDown.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
        experienceNumericUpDown.Size = new Size(160, 25);
        experienceNumericUpDown.ThousandsSeparator = true;
        experienceNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
        experienceNumericUpDown.ValueChanged += editor_ValueChanged;
        powerLabel.Anchor = AnchorStyles.Left;
        powerLabel.AutoSize = true;
        powerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        powerLabel.Text = "Power";
        powerNumericUpDown.Anchor = AnchorStyles.Left;
        powerNumericUpDown.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
        powerNumericUpDown.Size = new Size(120, 25);
        powerNumericUpDown.ValueChanged += editor_ValueChanged;
        teamLabel.Anchor = AnchorStyles.Left;
        teamLabel.AutoSize = true;
        teamLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        teamLabel.Text = "Team";
        teamNumericUpDown.Anchor = AnchorStyles.Left;
        teamNumericUpDown.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
        teamNumericUpDown.Size = new Size(120, 25);
        teamNumericUpDown.ValueChanged += editor_ValueChanged;
        readyLabel.Anchor = AnchorStyles.Left;
        readyLabel.AutoSize = true;
        readyLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        readyLabel.Text = "Ready";
        readyCheckBox.Anchor = AnchorStyles.Left;
        readyCheckBox.AutoSize = true;
        readyCheckBox.Checked = true;
        readyCheckBox.CheckState = CheckState.Checked;
        readyCheckBox.Text = "Listo al entrar";
        readyCheckBox.CheckedChanged += editor_ValueChanged;
        previewPanel.BackColor = Color.FromArgb(246, 241, 232);
        previewPanel.Controls.Add(previewValueLabel);
        previewPanel.Controls.Add(previewTitleLabel);
        previewPanel.Dock = DockStyle.Fill;
        previewPanel.Margin = new Padding(0, 8, 0, 8);
        previewPanel.Padding = new Padding(16);
        previewTitleLabel.AutoSize = true;
        previewTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        previewTitleLabel.ForeColor = Color.FromArgb(102, 84, 58);
        previewTitleLabel.Location = new Point(16, 16);
        previewTitleLabel.Text = "Vista previa en sala";
        previewValueLabel.AutoSize = true;
        previewValueLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        previewValueLabel.Location = new Point(16, 48);
        previewValueLabel.Text = "Selecciona un partner";
        actionPanel.Controls.Add(applyButton);
        actionPanel.Controls.Add(saveCatalogButton);
        actionPanel.Controls.Add(openFolderButton);
        actionPanel.Dock = DockStyle.Fill;
        actionPanel.FlowDirection = FlowDirection.LeftToRight;
        actionPanel.Margin = new Padding(0, 10, 0, 0);
        applyButton.BackColor = Color.FromArgb(35, 52, 68);
        applyButton.FlatAppearance.BorderSize = 0;
        applyButton.FlatStyle = FlatStyle.Flat;
        applyButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        applyButton.ForeColor = Color.White;
        applyButton.Size = new Size(130, 36);
        applyButton.Text = "Aplicar perfil";
        applyButton.UseVisualStyleBackColor = false;
        applyButton.Click += applyButton_Click;
        saveCatalogButton.BackColor = Color.FromArgb(120, 180, 156);
        saveCatalogButton.FlatAppearance.BorderSize = 0;
        saveCatalogButton.FlatStyle = FlatStyle.Flat;
        saveCatalogButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        saveCatalogButton.Size = new Size(160, 36);
        saveCatalogButton.Text = "Guardar catalogo";
        saveCatalogButton.UseVisualStyleBackColor = false;
        saveCatalogButton.Click += saveCatalogButton_Click;
        openFolderButton.BackColor = Color.FromArgb(242, 188, 90);
        openFolderButton.FlatAppearance.BorderSize = 0;
        openFolderButton.FlatStyle = FlatStyle.Flat;
        openFolderButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        openFolderButton.Size = new Size(160, 36);
        openFolderButton.Text = "Abrir carpeta";
        openFolderButton.UseVisualStyleBackColor = false;
        openFolderButton.Click += openFolderButton_Click;
        saveStatusLabel.AutoSize = true;
        saveStatusLabel.Dock = DockStyle.Top;
        saveStatusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        saveStatusLabel.ForeColor = Color.FromArgb(102, 84, 58);
        headerPanel.BackColor = Color.FromArgb(240, 136, 75);
        headerPanel.Controls.Add(pathValueLabel);
        headerPanel.Controls.Add(pathCaptionLabel);
        headerPanel.Controls.Add(subtitleLabel);
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 96;
        headerPanel.Margin = new Padding(0, 0, 0, 18);
        headerPanel.Padding = new Padding(24, 18, 24, 16);
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
        titleLabel.ForeColor = Color.White;
        titleLabel.Location = new Point(24, 12);
        titleLabel.Text = "Synthetic Partner Manager";
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 10F);
        subtitleLabel.ForeColor = Color.FromArgb(255, 244, 228);
        subtitleLabel.Location = new Point(28, 52);
        subtitleLabel.Text = "Crea, clona y guarda perfiles para los partners sinteticos del GameServer.";
        pathCaptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        pathCaptionLabel.AutoSize = true;
        pathCaptionLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        pathCaptionLabel.ForeColor = Color.FromArgb(255, 244, 228);
        pathCaptionLabel.Location = new Point(740, 16);
        pathCaptionLabel.Text = "Catalogo";
        pathValueLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        pathValueLabel.AutoEllipsis = true;
        pathValueLabel.Font = new Font("Segoe UI", 8F);
        pathValueLabel.ForeColor = Color.White;
        pathValueLabel.Location = new Point(520, 34);
        pathValueLabel.Size = new Size(400, 42);
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 700);
        Controls.Add(shellPanel);
        MinimumSize = new Size(960, 680);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Audition Synthetic Partner Manager";
        shellPanel.ResumeLayout(false);
        contentLayout.ResumeLayout(false);
        listPanel.ResumeLayout(false);
        listPanel.PerformLayout();
        listButtonsPanel.ResumeLayout(false);
        editorPanel.ResumeLayout(false);
        editorLayout.ResumeLayout(false);
        editorLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)genderNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)experienceNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)powerNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)teamNumericUpDown).EndInit();
        previewPanel.ResumeLayout(false);
        previewPanel.PerformLayout();
        actionPanel.ResumeLayout(false);
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ResumeLayout(false);
    }
}
