using System.Windows.Forms;

public class InputForm
{
    public static (string Category, string Name, bool IsUpdateOnly) ShowDialog()
    {
        string category = "";
        string name = "";
        bool updateOnly = false;

        var form = new Form
        {
            Width = 320,
            Height = 180,
            Text = "名刺登録",
            TopMost = true,
            StartPosition = FormStartPosition.CenterScreen,
            KeyPreview = true
        };

        var txtCategory = new TextBox { Left = 20, Top = 20, Width = 260, PlaceholderText = "Category (例: SW)" };
        var txtName = new TextBox { Left = 20, Top = 50, Width = 260, PlaceholderText = "Name" };

        var okButton = new Button { Text = "登録", Left = 20, Width = 80, Top = 90 };
        var updateButton = new Button { Text = "更新", Left = 110, Width = 80, Top = 90 };
        var cancelButton = new Button { Text = "キャンセル", Left = 200, Width = 80, Top = 90 };

        // 登録
        okButton.Click += (s, e) =>
        {
            category = txtCategory.Text;
            name = txtName.Text;
            form.Close();
        };

        // 更新（CSV→JSONだけ）
        updateButton.Click += (s, e) =>
        {
            updateOnly = true;
            form.Close();
        };

        // キャンセル
        cancelButton.Click += (s, e) =>
        {
            form.Close();
        };

        // Enterで登録
        form.AcceptButton = okButton;
        form.CancelButton = cancelButton;

        form.Shown += (s, e) =>
        {
            form.Activate();
            txtCategory.Focus();
        };

        form.Controls.Add(txtCategory);
        form.Controls.Add(txtName);
        form.Controls.Add(okButton);
        form.Controls.Add(updateButton);
        form.Controls.Add(cancelButton);

        form.ShowDialog();

        return (category, name, updateOnly);
    }
}