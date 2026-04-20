using System.Windows.Forms;

public class InputForm
{
    public static (string category, string name) ShowDialog()
    {
        string category = "";
        string name = "";

        var form = new Form
        {
            Width = 300,
            Height = 180,
            Text = "入力",
            TopMost = true,
            StartPosition = FormStartPosition.CenterScreen
        };

        var categoryBox = new TextBox
        {
            Left = 20,
            Top = 20,
            Width = 240,
            PlaceholderText = "Category (例: SW)"
        };

        var nameBox = new TextBox
        {
            Left = 20,
            Top = 60,
            Width = 240,
            PlaceholderText = "Name"
        };

        var okButton = new Button
        {
            Text = "OK",
            Left = 60,
            Width = 80,
            Top = 100
        };

        var cancelButton = new Button
        {
            Text = "キャンセル",
            Left = 150,
            Width = 80,
            Top = 100
        };

        // -------------------------
        // OK処理
        // -------------------------
        okButton.Click += (s, e) =>
        {
            category = categoryBox.Text;
            name = nameBox.Text;
            form.DialogResult = DialogResult.OK;
            form.Close();
        };

        // -------------------------
        // キャンセル
        // -------------------------
        cancelButton.Click += (s, e) =>
        {
            form.DialogResult = DialogResult.Cancel;
            form.Close();
        };

        // ★これが重要（Enter / Esc）
        form.AcceptButton = okButton;
        form.CancelButton = cancelButton;

        // ★フォーカス強制
        form.Shown += (s, e) =>
        {
            nameBox.Focus();   // ←ここ重要
        };

        form.Controls.Add(categoryBox);
        form.Controls.Add(nameBox);
        form.Controls.Add(okButton);
        form.Controls.Add(cancelButton);

        form.ShowDialog();

        return (category, name);
    }
}