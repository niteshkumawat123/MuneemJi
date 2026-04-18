using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class StateController : Controller
    {
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public StateController()
        {

        }
        public List<StateModel> StateDropDown()
        {
            List<StateModel> Model = new List<StateModel>();
            using (var Connection = new NpgsqlConnection(_connectionString))
            {

                var QueryString = "select id as stateid , name  from states ";



                Model = Connection.QuerySql<StateModel>(QueryString).ToList();

            }
            return Model;
        }

    }

}





