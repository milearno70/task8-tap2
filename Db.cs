using Microsoft.Data.SqlClient;

namespace AdoNetTasks;

static class Db
{

    public const string ConnStr =
        @"Server=DESKTOP-6Q4JS4E\SQLEXPRESS01;Database=SchoolDB;" +
        "Trusted_Connection=True;TrustServerCertificate=True;";
}


class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Credits { get; set; }
}


class CourseRepository
{
    public List<Course> GetAll()
    {
        var list = new List<Course>();
        using (var con = new SqlConnection(Db.ConnStr))
        {
            var cmd = new SqlCommand("SELECT Id, Title, Credits FROM Courses ORDER BY Title", con);
            con.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var c = new Course();
                    c.Id = r.GetInt32(0);
                    c.Title = r.GetString(1);
                    c.Credits = r.GetInt32(2);
                    list.Add(c);
                }
            }
        }
        return list;
    }

    public void Add(Course c)
    {
        using (var con = new SqlConnection(Db.ConnStr))
        {
            var cmd = new SqlCommand("INSERT INTO Courses (Title, Credits) VALUES (@t, @c)", con);
            cmd.Parameters.AddWithValue("@t", c.Title);
            cmd.Parameters.AddWithValue("@c", c.Credits);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void Update(Course c)
    {
        using (var con = new SqlConnection(Db.ConnStr))
        {
            var cmd = new SqlCommand("UPDATE Courses SET Title = @t, Credits = @c WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@t", c.Title);
            cmd.Parameters.AddWithValue("@c", c.Credits);
            cmd.Parameters.AddWithValue("@id", c.Id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }


    public void Delete(int id)
    {
        using (var con = new SqlConnection(Db.ConnStr))
        {
            var cmd = new SqlCommand("DELETE FROM Courses WHERE Id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public int Count()
    {
        using (var con = new SqlConnection(Db.ConnStr))
        {
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Courses", con);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
