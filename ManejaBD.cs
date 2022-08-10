using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ManejaXML
{
    public class ManejaBD
    {
        public SqlConnection conexionSql()
        {
            try
            {
                string cadenaConexion = "Data Source=LTELMXCFLORES\\SQL2017;Initial Catalog=bmnpad02;User ID=sa;Password=telepro";
                return new SqlConnection(cadenaConexion);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int obtenerIdPerfil(string nombrePerfil)
        {
            SqlConnection sqlConn = conexionSql();

            if (nombrePerfil.Equals(""))
            {
                return 0;
            }

            int idPerfil = 0;

            using (sqlConn)
            {
                sqlConn.Open();

                SqlCommand cmd = new SqlCommand("SELECT PDK_ID_PERFIL FROM PDK_PERFIL WHERE PDK_PER_NOMBRE = '" + nombrePerfil + "'", sqlConn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    idPerfil = Convert.ToInt32(dr["PDK_ID_PERFIL"].ToString());
                }

                return idPerfil;
            }
        }

        //NO SE UTILIZA FUNCIÓN
        //public int comprobarBalanceo(int idPerfil)
        //{
        //    SqlConnection sqlConn = conexionSql();
        //    int balancea = 0;

        //    using (sqlConn)
        //    {
        //        sqlConn.Open();

        //        SqlCommand cmd = new SqlCommand("SELECT PDK_BALANCEA FROM PDK_PERFIL WHERE PDK_ID_PERFIL = '" + idPerfil + "'", sqlConn);
        //        SqlDataReader dr = cmd.ExecuteReader();

        //        if (dr.Read())
        //        {
        //            balancea = Convert.ToInt32(dr["PDK_BALANCEA"]);
        //        }

        //        return balancea;
        //    }
        //}
    }
}

