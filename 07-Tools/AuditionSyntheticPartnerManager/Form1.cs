namespace AuditionSyntheticPartnerManager;

public partial class Form1 : Form
{
    private readonly PartnerCatalogService _catalogService = new();
    private readonly BindingSource _profilesSource = new();
    private readonly BindingSource _partnerSelectionSource = new();
    private List<SyntheticPartnerProfile> _profiles = [];
    private SyntheticPartnerSettings _settings = SyntheticPartnerSettings.Default;
    private bool _updatingEditors;
    private CheckBox? _autoAddCheckBox;
    private ComboBox? _partnerSelectionComboBox;

    public Form1()
    {
        InitializeComponent();
        ConfigureUi();
        LoadCatalog();
    }

    private void ConfigureUi()
    {
        partnerListBox.DisplayMember = nameof(SyntheticPartnerProfile.Nickname);
        partnerListBox.DataSource = _profilesSource;
        InstallSyntheticControlPanel();
        pathValueLabel.Text = _catalogService.CatalogPath;
        subtitleLabel.Text = "Administra perfiles y decide desde esta herramienta si el GameServer agrega un partner sintetico al crear sala.";
        saveStatusLabel.Text = string.Empty;
    }

    private void LoadCatalog()
    {
        _profiles = _catalogService.LoadProfiles();
        _settings = _catalogService.LoadSettings();
        RefreshBinding();
        RefreshPartnerSelectionOptions();
        ApplySettingsToUi();

        if (_profiles.Count > 0)
        {
            partnerListBox.SelectedIndex = 0;
        }
    }

    private void RefreshBinding()
    {
        _profilesSource.DataSource = null;
        _profilesSource.DataSource = _profiles;
        partnerListBox.DisplayMember = nameof(SyntheticPartnerProfile.Nickname);
    }

    private void InstallSyntheticControlPanel()
    {
        Panel controlPanel = new()
        {
            Dock = DockStyle.Top,
            Height = 92,
            BackColor = Color.FromArgb(246, 241, 232),
            Padding = new Padding(18, 14, 18, 12)
        };

        Label title = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Text = "Insercion automatica en sala"
        };

        _autoAddCheckBox = new CheckBox
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Location = new Point(0, 30),
            Text = "Agregar partner sintetico al crear sala"
        };
        _autoAddCheckBox.CheckedChanged += syntheticControl_ValueChanged;

        Label modeLabel = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Location = new Point(360, 12),
            Text = "Perfil usado"
        };

        _partnerSelectionComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9F),
            Location = new Point(360, 32),
            Size = new Size(250, 23),
            DisplayMember = nameof(PartnerSelectionOption.Label),
            ValueMember = nameof(PartnerSelectionOption.UserId),
            DataSource = _partnerSelectionSource
        };
        _partnerSelectionComboBox.SelectedIndexChanged += syntheticControl_ValueChanged;

        controlPanel.Controls.Add(title);
        controlPanel.Controls.Add(_autoAddCheckBox);
        controlPanel.Controls.Add(modeLabel);
        controlPanel.Controls.Add(_partnerSelectionComboBox);
        editorPanel.Controls.Add(controlPanel);
        editorPanel.Controls.SetChildIndex(controlPanel, 0);
    }

    private void RefreshPartnerSelectionOptions()
    {
        List<PartnerSelectionOption> options =
        [
            new PartnerSelectionOption(string.Empty, "Rotacion secuencial")
        ];

        foreach (SyntheticPartnerProfile profile in _profiles)
        {
            options.Add(new PartnerSelectionOption(profile.UserId, $"{profile.Nickname} ({profile.UserId})"));
        }

        _partnerSelectionSource.DataSource = options;
    }

    private void ApplySettingsToUi()
    {
        if (_autoAddCheckBox is null || _partnerSelectionComboBox is null)
        {
            return;
        }

        _updatingEditors = true;
        _autoAddCheckBox.Checked = _settings.Enabled;

        object selectedItem = _partnerSelectionSource.List
            .Cast<PartnerSelectionOption>()
            .FirstOrDefault(option => string.Equals(option.UserId, _settings.SelectedProfileUserId, StringComparison.OrdinalIgnoreCase))
            ?? _partnerSelectionSource.List.Cast<PartnerSelectionOption>().First();

        _partnerSelectionComboBox.SelectedItem = selectedItem;
        _partnerSelectionComboBox.Enabled = _autoAddCheckBox.Checked;
        _updatingEditors = false;
    }

    private void CaptureSettingsFromUi()
    {
        if (_autoAddCheckBox is null || _partnerSelectionComboBox?.SelectedItem is not PartnerSelectionOption selection)
        {
            _settings = SyntheticPartnerSettings.Default;
            return;
        }

        _settings = new SyntheticPartnerSettings
        {
            Enabled = _autoAddCheckBox.Checked,
            SelectedProfileUserId = selection.UserId
        };
    }

    private void partnerListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (partnerListBox.SelectedItem is not SyntheticPartnerProfile profile)
        {
            return;
        }

        _updatingEditors = true;
        nicknameTextBox.Text = profile.Nickname;
        userIdTextBox.Text = profile.UserId;
        genderNumericUpDown.Value = profile.Gender;
        experienceNumericUpDown.Value = Math.Min(experienceNumericUpDown.Maximum, profile.Experience);
        powerNumericUpDown.Value = profile.Power;
        teamNumericUpDown.Value = profile.Team;
        readyCheckBox.Checked = profile.Ready;
        previewValueLabel.Text = $"{profile.Nickname}  |  ID {profile.UserId}  |  EXP {profile.Experience:N0}  |  Power {profile.Power}";
        _updatingEditors = false;
    }

    private void addButton_Click(object? sender, EventArgs e)
    {
        SyntheticPartnerProfile profile = new()
        {
            Nickname = $"partner{_profiles.Count + 1}",
            UserId = $"partner{_profiles.Count + 1}",
            Gender = 1,
            Experience = 1,
            Power = 0,
            Ready = true,
            Team = 0
        };

        _profiles.Add(profile);
        RefreshBinding();
        RefreshPartnerSelectionOptions();
        ApplySettingsToUi();
        partnerListBox.SelectedItem = profile;
        saveStatusLabel.Text = "Perfil agregado. Guarda el catalogo para aplicarlo al GameServer.";
    }

    private void cloneButton_Click(object? sender, EventArgs e)
    {
        if (partnerListBox.SelectedItem is not SyntheticPartnerProfile profile)
        {
            return;
        }

        SyntheticPartnerProfile clone = profile.Clone();
        clone.Nickname = $"{profile.Nickname}_copy";
        clone.UserId = $"{profile.UserId}_copy";
        _profiles.Add(clone);
        RefreshBinding();
        RefreshPartnerSelectionOptions();
        ApplySettingsToUi();
        partnerListBox.SelectedItem = clone;
        saveStatusLabel.Text = "Perfil clonado. Ajusta los datos y guarda el catalogo.";
    }

    private void deleteButton_Click(object? sender, EventArgs e)
    {
        if (partnerListBox.SelectedItem is not SyntheticPartnerProfile profile)
        {
            return;
        }

        if (_profiles.Count == 1)
        {
            MessageBox.Show(this, "Debe existir al menos un partner en el catalogo.", "Catalogo protegido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _profiles.Remove(profile);
        RefreshBinding();
        if (string.Equals(_settings.SelectedProfileUserId, profile.UserId, StringComparison.OrdinalIgnoreCase))
        {
            _settings.SelectedProfileUserId = string.Empty;
        }
        RefreshPartnerSelectionOptions();
        ApplySettingsToUi();
        partnerListBox.SelectedIndex = Math.Max(0, partnerListBox.SelectedIndex);
        saveStatusLabel.Text = "Perfil eliminado. Guarda el catalogo para persistir el cambio.";
    }

    private void applyButton_Click(object? sender, EventArgs e)
    {
        if (partnerListBox.SelectedItem is not SyntheticPartnerProfile profile)
        {
            return;
        }

        if (!TryApplyEditors(profile))
        {
            return;
        }

        RefreshBinding();
        partnerListBox.SelectedItem = profile;
        saveStatusLabel.Text = "Perfil actualizado en memoria. Guarda el catalogo para persistirlo.";
    }

    private void saveCatalogButton_Click(object? sender, EventArgs e)
    {
        if (partnerListBox.SelectedItem is SyntheticPartnerProfile profile && !TryApplyEditors(profile))
        {
            return;
        }

        CaptureSettingsFromUi();
        _catalogService.SaveProfiles(_profiles);
        _catalogService.SaveSettings(_settings);
        saveStatusLabel.Text = $"Catalogo y control guardados en {_catalogService.CatalogPath}";
    }

    private bool TryApplyEditors(SyntheticPartnerProfile profile)
    {
        string nickname = nicknameTextBox.Text.Trim();
        string userId = userIdTextBox.Text.Trim();

        if (nickname.Length == 0 || userId.Length == 0)
        {
            MessageBox.Show(this, "Nickname y User ID son obligatorios.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        profile.Nickname = nickname;
        profile.UserId = userId;
        profile.Gender = (byte)genderNumericUpDown.Value;
        profile.Experience = (uint)experienceNumericUpDown.Value;
        profile.Power = (byte)powerNumericUpDown.Value;
        profile.Team = (byte)teamNumericUpDown.Value;
        profile.Ready = readyCheckBox.Checked;
        previewValueLabel.Text = $"{profile.Nickname}  |  ID {profile.UserId}  |  EXP {profile.Experience:N0}  |  Power {profile.Power}";
        return true;
    }

    private void openFolderButton_Click(object? sender, EventArgs e)
    {
        string? folderPath = Path.GetDirectoryName(_catalogService.CatalogPath);
        if (string.IsNullOrWhiteSpace(folderPath))
        {
            return;
        }

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = folderPath,
            UseShellExecute = true
        });
    }

    private void editor_ValueChanged(object? sender, EventArgs e)
    {
        if (_updatingEditors)
        {
            return;
        }

        saveStatusLabel.Text = "Hay cambios sin guardar en el perfil o en el catalogo.";
    }

    private void syntheticControl_ValueChanged(object? sender, EventArgs e)
    {
        if (_updatingEditors)
        {
            return;
        }

        if (_partnerSelectionComboBox is not null && _autoAddCheckBox is not null)
        {
            _partnerSelectionComboBox.Enabled = _autoAddCheckBox.Checked;
        }

        saveStatusLabel.Text = "Cambiaste el control de insercion automatica. Guarda para aplicarlo al GameServer.";
    }
}

internal sealed record PartnerSelectionOption(string UserId, string Label);
