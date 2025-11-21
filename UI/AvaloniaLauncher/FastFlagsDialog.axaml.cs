using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KSCSharp.AvaloniaLauncher;

public partial class FastFlagsDialog : Window
{
    private readonly List<FlagItem> _items;
    private DataGrid FlagsGrid => this.FindControl<DataGrid>("FlagsGrid")!;
    private TextBox TxtKey => this.FindControl<TextBox>("TxtKey")!;
    private TextBox TxtValue => this.FindControl<TextBox>("TxtValue")!;
    private Button BtnAddOrUpdate => this.FindControl<Button>("BtnAddOrUpdate")!;
    private Button BtnSave => this.FindControl<Button>("BtnSave")!;
    private Button BtnCancel => this.FindControl<Button>("BtnCancel")!;

    public record Result(bool Saved, Dictionary<string, object> Flags);
    public record FlagItem(string Key, object Value);

    public FastFlagsDialog(Dictionary<string, object> flags)
    {
        InitializeComponent();

        _items = flags.Select(kv => new FlagItem(kv.Key, kv.Value)).ToList();
        FlagsGrid.Items = _items;

        BtnAddOrUpdate.Click += BtnAddOrUpdate_Click;
        BtnSave.Click += BtnSave_Click;
        BtnCancel.Click += BtnCancel_Click;
    }

    private void BtnAddOrUpdate_Click(object? sender, RoutedEventArgs e)
    {
        var key = (TxtKey.Text ?? "").Trim();
        var raw = TxtValue.Text ?? "";
        if (string.IsNullOrWhiteSpace(key)) return;

        object val = KSCSharp.Core.FastFlagsManager.AutoDetectValue(raw);

        var existing = _items.FirstOrDefault(i => string.Equals(i.Key, key, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            _items.Remove(existing);
        }
        _items.Add(new FlagItem(key, val));
        FlagsGrid.Items = null;
        FlagsGrid.Items = _items;
        TxtKey.Text = "";
        TxtValue.Text = "";
    }

    private void BtnSave_Click(object? sender, RoutedEventArgs e)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in _items)
            dict[item.Key] = item.Value;
        Close(new Result(true, dict));
    }

    private void BtnCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(new Result(false, new Dictionary<string, object>()));
    }
}
