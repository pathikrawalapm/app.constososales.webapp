using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;


namespace app.constososales.webapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfiguration _configuration;

        private static string Host = "";
        private static string User = "";
        private static string DBname = "";
        private static string Password = "";
        private static string Port = "";

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnGet()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            //var Host = MyConfig.GetValue<string>("AppSettings:Host");
            //var User = MyConfig.GetValue<string>("AppSettings:User");
            //var DBname = MyConfig.GetValue<string>("AppSettings:DBname");
            //var Password = MyConfig.GetValue<string>("AppSettings:Password");
            //var Port = MyConfig.GetValue<string>("AppSettings:Port");

            var Host = _configuration.GetConnectionString("Host");
            var User = _configuration.GetConnectionString("User");
            var DBname = _configuration.GetConnectionString("DBname");
            var Password = _configuration.GetConnectionString("Password");
            var Port = _configuration.GetConnectionString("Port");

          
            string connString =
               String.Format(
                   "Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                   Host,
                   User,
                   DBname,
                   Port,
                   Password);

            using (var conn = new NpgsqlConnection(connString))

            {
                Console.Out.WriteLine("Opening the connection");
                conn.Open();

                using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS inventory", conn))
                {
                    command.ExecuteNonQuery();
                   // Console.Out.WriteLine("Finished dropping table (if existed)");

                }

                using (var command = new NpgsqlCommand("CREATE TABLE inventory(id serial PRIMARY KEY, name VARCHAR(50), quantity INTEGER)", conn))
                {
                    command.ExecuteNonQuery();
                    //Console.Out.WriteLine("Finished creating table");
                }

                using (var command = new NpgsqlCommand("INSERT INTO inventory (name, quantity) VALUES (@n1, @q1), (@n2, @q2), (@n3, @q3)", conn))
                {
                    command.Parameters.AddWithValue("n1", "banana11");
                    command.Parameters.AddWithValue("q1", 150);
                    command.Parameters.AddWithValue("n2", "orange222");
                    command.Parameters.AddWithValue("q2", 154);
                    command.Parameters.AddWithValue("n3", "apple22");
                    command.Parameters.AddWithValue("q3", 100);

                    int nRows = command.ExecuteNonQuery();
                    Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                }
            }

            using (var conn = new NpgsqlConnection(connString))
            {

               // Console.Out.WriteLine("Opening connection");
                conn.Open();


                using (var command = new NpgsqlCommand("SELECT * FROM inventory", conn))
                {

                    var reader = command.ExecuteReader();
                   
                    reader.Close();
                }
            }


           // Console.WriteLine("Press RETURN to exit");
           // Console.ReadLine();
        }
    }
}
