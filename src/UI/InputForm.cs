using System.Windows.Forms;

public class InputForm
{
    public static string ShowDialog()
    {
        string result = "";

        var form = new Form
        {
            Width = 300,
            Height = 150,
            Text = "名前入力",
            TopMost = true,
            StartPosition = FormStartPosition.CenterScreen,
            KeyPreview = true // Enter / Escを拾う
        };

        var textBox = new TextBox
        {
            Left = 20,
            Top = 20,
            Width = 240
        };

        var okButton = new Button
        {
            Text = "OK",
            Left = 60,
            Width = 80,
            Top = 60
        };

        var cancelButton = new Button
        {
            Text = "キャンセル",
            Left = 150,
            Width = 80,
            Top = 60
        };

        // OK処理
        okButton.Click += (s, e) =>
        {
            result = textBox.Text;
            form.Close();
        };

        // キャンセル処理
        cancelButton.Click += (s, e) =>
        {
            result = "";
            form.Close();
        };

        // EnterキーでOK
        form.AcceptButton = okButton;

        // Escキーでキャンセル
        form.CancelButton = cancelButton;

        // 表示時に最前面＋フォーカス
        form.Shown += (s, e) =>
        {
            form.Activate();
            form.BringToFront();
            textBox.Focus();
        };

        form.Controls.Add(textBox);
        form.Controls.Add(okButton);
        form.Controls.Add(cancelButton);

        form.ShowDialog();

        return result;
    }
}