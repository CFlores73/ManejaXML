using System.Data.SqlClient;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ManejaXML
{
    class XmlLINQ
    {
        List<string> thirdPartys = new List<string>();

        public void ProcesarXML(string rutaXML)
        {
            XDocument Xml = XDocument.Load(rutaXML);

            //======================================================= VARIABLES PARA PARA BD            
            string contractIe = ""; //1 por documento            

            var addressListElements = new List<string> { "thirdParty", "address1", "address2", "addressNr", "canton", "countryCode", "county", "district", "entryDate", "postalCode", "statusCode", "town", "typaddrCode" };
            var addressListDictionary = new Dictionary<string, string>();

            var bankAccountReferenceListElements = new List<string> { "thirdParty", "bankRefNr", "account", "address", "bank", "bankRefTypeCode", "barFormatCode", "branch", "countryCode", "origBankRefNr", "statusCode", "substitutionBankRefNr" };
            var bankAccountReferenceListDictionary = new Dictionary<string, string>();

            var partyContactListElements = new List<string> { "thirdParty", "commrefId", "email", "forname", "mobilePhone", "name", "phone", "referenceCode", "representative" };
            var partyContactListDictionary = new Dictionary<string, string>();

            var roleListElements = new List<string> { "thirdParty", "roleCode", "creationDate", "currencyCode", "invoicingAddressNr", "payBankRefNr", "paymentAddressNr", "paymentCode" };
            var roleListDictionary = new Dictionary<string, string>();
            //=======================================================

            XElement returnNode = Xml.XPathSelectElements("//return").FirstOrDefault();
            contractIe = returnNode.Element("contractIe").Value;

            procesarElemento(Xml, "addressList", addressListElements, addressListDictionary);
            procesarElemento(Xml, "bankAccountReferenceList", bankAccountReferenceListElements, bankAccountReferenceListDictionary);
            procesarElemento(Xml, "partyContactList", partyContactListElements, partyContactListDictionary);
            procesarElemento(Xml, "roleList", roleListElements, roleListDictionary);

            //Se guardan las relaciones para cada thirdParty
            foreach(string thirdParty in thirdPartys)
            {
                guardarRelacion(contractIe, "qwe", thirdParty, 999);
            }            
        }

        public void guardarRelacion(string contractIe, string CTO_FL_CVE, string thirdParty, int PDK_ID_CLIENTE)
        {
            SqlConnection sqlConn = conexionSql();

            string query = "INSERT INTO PDK_REL_CLIENTE_EKIP";
            query += " (contractIe, CTO_FL_CVE, thirdParty, PDK_ID_CLIENTE)";
            query += " VALUES('" + contractIe + "','" + CTO_FL_CVE + "','" + thirdParty + "','" + PDK_ID_CLIENTE + "')";

            try
            {
                using (sqlConn)
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(query, sqlConn);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
            catch (Exception ex) { }
        }

        public void procesarElemento(XDocument Xml, string nombreXElement, List<string> listaElementos, Dictionary<string, string> diccionario)
        {
            foreach (XElement xElement in Xml.XPathSelectElements("//" + nombreXElement))
            {
                foreach (string element in listaElementos)
                {
                    try
                    {
                        diccionario.Add(element, xElement.Element(element).Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("falló la inserción del elemento " + element);
                    }
                }

                //Se obtienen los valores separados del diccionario
                guardarValores(diccionario, nombreXElement);
            }
        }
        
        public void guardarValores(Dictionary<string, string> diccionario, string nombreDiccionario)
        {
            SqlConnection sqlConn = conexionSql();
            sqlConn.Open();            

            switch (nombreDiccionario)
            {
                case ("addressList"):
                    string thirdPartyAL = diccionario.ContainsKey("thirdParty") ? diccionario["thirdParty"] : "";
                    string address1 = diccionario.ContainsKey("address1") ? diccionario["address1"] : "";
                    string address2 = diccionario.ContainsKey("address2") ? diccionario["address2"] : "";
                    int    addressNr = diccionario.ContainsKey("addressNr") ? Convert.ToInt32(diccionario["addressNr"]) : 0;
                    string canton = diccionario.ContainsKey("canton") ? diccionario["canton"] : "";
                    string countryCode = diccionario.ContainsKey("countryCode") ? diccionario["countryCode"] : "";
                    string county = diccionario.ContainsKey("county") ? diccionario["county"] : "";
                    string district = diccionario.ContainsKey("district") ? diccionario["district"] : "";
                    string entryDate = diccionario.ContainsKey("entryDate") ? diccionario["entryDate"] : "";
                    string postalCode = diccionario.ContainsKey("postalCode") ? diccionario["postalCode"] : "";
                    string statusCode = diccionario.ContainsKey("statusCode") ? diccionario["statusCode"] : "";
                    string town = diccionario.ContainsKey("town") ? diccionario["town"] : "";
                    string typaddrCode = diccionario.ContainsKey("typaddrCode") ? diccionario["typaddrCode"] : "";

                    //Se agrega el thirdParty para la relación
                    if (!thirdPartys.Contains(thirdPartyAL)) thirdPartys.Add(thirdPartyAL);
                    
                    using (sqlConn)
                    {
                        SqlCommand cmd = new SqlCommand("stp_crud_PDK_EKIP_ADDRESS_LIST", sqlConn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@thirdParty", thirdPartyAL);
                        cmd.Parameters.AddWithValue("@address1", address1);
                        cmd.Parameters.AddWithValue("@address2", address2);
                        cmd.Parameters.AddWithValue("@addressNr", addressNr);
                        cmd.Parameters.AddWithValue("@canton", canton);
                        cmd.Parameters.AddWithValue("@countryCode", countryCode);
                        cmd.Parameters.AddWithValue("@county", county);
                        cmd.Parameters.AddWithValue("@district", district);
                        cmd.Parameters.AddWithValue("@entryDate", entryDate);
                        cmd.Parameters.AddWithValue("@postalCode", postalCode);
                        cmd.Parameters.AddWithValue("@statusCode", statusCode);
                        cmd.Parameters.AddWithValue("@town", town);
                        cmd.Parameters.AddWithValue("@typaddrCode", typaddrCode);

                        cmd.ExecuteNonQuery();                       
                    }
                    //Console.WriteLine("=========== ADDRESS LIST ===========");
                    //Console.WriteLine(thirdPartyAL);
                    //Console.WriteLine(address1);
                    //Console.WriteLine(address2);
                    //Console.WriteLine(addressNr);
                    //Console.WriteLine(canton);
                    //Console.WriteLine(countryCode);
                    //Console.WriteLine(county);
                    //Console.WriteLine(district);
                    //Console.WriteLine(entryDate);
                    //Console.WriteLine(postalCode);
                    //Console.WriteLine(statusCode);
                    //Console.WriteLine(town);
                    //Console.WriteLine(typaddrCode);                    
                    break;
                case ("bankAccountReferenceList"):                    
                    string thirdPartyBA = diccionario.ContainsKey("thirdParty") ? diccionario["thirdParty"] : "";
                    int bankRefNr = diccionario.ContainsKey("bankRefNr") ? Convert.ToInt32(diccionario["bankRefNr"]) : 0;
                    string account = diccionario.ContainsKey("account") ? diccionario["account"] : "";
                    string address = diccionario.ContainsKey("address") ? diccionario["address"] : "";
                    string bank = diccionario.ContainsKey("bank") ? diccionario["bank"] : "";
                    string bankRefTypeCode = diccionario.ContainsKey("bankRefTypeCode") ? diccionario["bankRefTypeCode"] : "";
                    string barFormatCode = diccionario.ContainsKey("barFormatCode") ? diccionario["barFormatCode"] : "";
                    string branch = diccionario.ContainsKey("branch") ? diccionario["branch"] : "";
                    string countryCodeBA = diccionario.ContainsKey("countryCode") ? diccionario["countryCode"] : "";
                    string origBankRefNr = diccionario.ContainsKey("origBankRefNr") ? diccionario["origBankRefNr"] : "";
                    string statusCodeBA = diccionario.ContainsKey("statusCode") ? diccionario["statusCode"] : "";
                    string substitutionBankRefNr = diccionario.ContainsKey("substitutionBankRefNr") ? diccionario["substitutionBankRefNr"] : "";

                    //Se agrega el thirdParty para la relación
                    if (!thirdPartys.Contains(thirdPartyBA)) thirdPartys.Add(thirdPartyBA);

                    //Console.WriteLine("=========== BANK ACCOUNT REFERENCE LIST ===========");
                    //Console.WriteLine(thirdPartyBA);
                    //Console.WriteLine(bankRefNr);
                    //Console.WriteLine(account);
                    //Console.WriteLine(address);
                    //Console.WriteLine(bank);
                    //Console.WriteLine(bankRefTypeCode);
                    //Console.WriteLine(barFormatCode);
                    //Console.WriteLine(branch);
                    //Console.WriteLine(countryCodeBA);
                    //Console.WriteLine(origBankRefNr);
                    //Console.WriteLine(statusCodeBA);
                    //Console.WriteLine(substitutionBankRefNr);
                    break;
                case ("partyContactList"):
                    string thirdPartyCL = diccionario.ContainsKey("thirdParty") ? diccionario["thirdParty"] : "";
                    string commrefId = diccionario.ContainsKey("commrefId") ? diccionario["commrefId"] : "";
                    string email = diccionario.ContainsKey("email") ? diccionario["email"] : "";
                    string forname = diccionario.ContainsKey("forname") ? diccionario["forname"] : "";
                    string mobilePhone = diccionario.ContainsKey("mobilePhone") ? diccionario["mobilePhone"] : "";
                    string name = diccionario.ContainsKey("name") ? diccionario["name"] : "";
                    string phone = diccionario.ContainsKey("phone") ? diccionario["phone"] : "";
                    string referenceCode = diccionario.ContainsKey("referenceCode") ? diccionario["referenceCode"] : "";
                    string representative = diccionario.ContainsKey("representative") ? diccionario["representative"] : "";

                    //Se agrega el thirdParty para la relación
                    if (!thirdPartys.Contains(thirdPartyCL)) thirdPartys.Add(thirdPartyCL);

                    //Console.WriteLine("=========== PARTY CONTACT LIST ===========");
                    //Console.WriteLine(thirdPartyCL);
                    //Console.WriteLine(commrefId);
                    //Console.WriteLine(email);
                    //Console.WriteLine(forname);
                    //Console.WriteLine(mobilePhone);
                    //Console.WriteLine(name);
                    //Console.WriteLine(phone);
                    //Console.WriteLine(referenceCode);
                    //Console.WriteLine(representative);
                    break;
                case ("roleList"):
                    string thirdPartyRL = diccionario.ContainsKey("thirdParty") ? diccionario["thirdParty"] : "";
                    string roleCode = diccionario.ContainsKey("roleCode") ? diccionario["roleCode"] : "";
                    string creationDate = diccionario.ContainsKey("creationDate") ? diccionario["creationDate"] : "";
                    string currencyCode = diccionario.ContainsKey("currencyCode") ? diccionario["currencyCode"] : "";
                    int invoicingAddressNr = diccionario.ContainsKey("invoicingAddressNr") ? Convert.ToInt32(diccionario["invoicingAddressNr"]) : 0;
                    int payBankRefNr = diccionario.ContainsKey("payBankRefNr") ? Convert.ToInt32(diccionario["payBankRefNr"]) : 0;
                    int paymentAddressNr = diccionario.ContainsKey("paymentAddressNr") ? Convert.ToInt32(diccionario["paymentAddressNr"]) : 0;
                    string paymentCode = diccionario.ContainsKey("paymentCode") ? diccionario["paymentCode"] : "";

                    //Se agrega el thirdParty para la relación
                    if (!thirdPartys.Contains(thirdPartyRL)) thirdPartys.Add(thirdPartyRL);

                    //Console.WriteLine("=========== ROLE LIST ===========");
                    //Console.WriteLine(thirdPartyRL);
                    //Console.WriteLine(roleCode);
                    //Console.WriteLine(creationDate);
                    //Console.WriteLine(currencyCode);
                    //Console.WriteLine(invoicingAddressNr);
                    //Console.WriteLine(payBankRefNr);
                    //Console.WriteLine(paymentAddressNr);
                    //Console.WriteLine(paymentCode);
                    break;
            }

            sqlConn.Close();
            diccionario.Clear();
        }

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

        //IMPRIMIR DICCIONARIO
        public void imprimirDiccionario(Dictionary<string, string> diccionario)
        {
            foreach (KeyValuePair<string, string> entrada in diccionario)
            {
                Console.WriteLine("Key: {0} - Valor: {1}", entrada.Key, entrada.Value);
            }
            Console.WriteLine("\n");

            diccionario.Clear();
        }
    }
}
