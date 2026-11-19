using Microsoft.Data.SqlClient;

namespace AdoNetTasks;


public partial class Form1 : Form
{
    CourseRepository repo = new CourseRepository();
    bool loading = false;

    public Form1()
    {
        InitializeComponent();
    }

    void Form1_Load(object sender, EventArgs e)
    {
        LoadList();
    }

    void LoadList()
    {
        try
        {
            loading = true;
            dgv.DataSource = repo.GetAll();
            dgv.Columns["Id"].Visible = false;
            lblCount.Text = "Fənlərin sayı: " + repo.Count();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Verilənlər bazası xətası: " + ex.Message);
        }
        finally
        {
            loading = false;
        }
    }


    void txtCredits_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            e.Handled = true;
    }

    void dgv_SelectionChanged(object sender, EventArgs e)
    {
        if (loading) return;

        var c = dgv.CurrentRow?.DataBoundItem as Course;
        if (c == null) return;

        txtTitle.Text = c.Title;
        txtCredits.Text = c.Credits.ToString();
    }


    bool ReadInput(out string title, out int credits)
    {
        title = txtTitle.Text.Trim();
        credits = 0;

        if (title == "")
        {
            MessageBox.Show("Fənnin adını yazın.");
            return false;
        }
        if (!int.TryParse(txtCredits.Text, out credits) || credits < 1 || credits > 10)
        {
            MessageBox.Show("Kredit 1 ilə 10 arasında olmalıdır.");
            return false;
        }
        return true;
    }

    void btnAdd_Click(object sender, EventArgs e)
    {
        string title;
        int credits;
        if (!ReadInput(out title, out credits)) return;

        try
        {
            var c = new Course();
            c.Title = title;
            c.Credits = credits;
            repo.Add(c);
            LoadList();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Verilənlər bazası xətası: " + ex.Message);
        }
    }

    void btnUpdate_Click(object sender, EventArgs e)
    {
        var selected = dgv.CurrentRow?.DataBoundItem as Course;
        if (selected == null)
        {
            MessageBox.Show("Düzəltmək üçün fənn seçin.");
            return;
        }

        string title;
        int credits;
        if (!ReadInput(out title, out credits)) return;

        try
        {
            var c = new Course();
            c.Id = selected.Id;
            c.Title = title;
            c.Credits = credits;
            repo.Update(c);
            LoadList();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Verilənlər bazası xətası: " + ex.Message);
        }
    }

    void btnDelete_Click(object sender, EventArgs e)
    {
        var selected = dgv.CurrentRow?.DataBoundItem as Course;
        if (selected == null)
        {
            MessageBox.Show("Silmək üçün fənn seçin.");
            return;
        }

        if (MessageBox.Show("\"" + selected.Title + "\" fənni silinsin?", "Təsdiq",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
            return;

        try
        {
            repo.Delete(selected.Id);
            LoadList();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {

            MessageBox.Show("Bu fənni silmək olmaz: ona aid qiymətlər var. Əvvəlcə həmin qiymətləri silin.");
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Verilənlər bazası xətası: " + ex.Message);
        }
    }
}
