using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sage.SData.Client;
using Sage.SData.Client.Extensions;
using Sage.SData.Client.Core;
using Sage.SData.Client.Atom;

namespace SdataNestedTest
{
    public partial class Form1 : Form
    { 
        
        Sage.SData.Client.Core.SDataService _service;
    

        private void InitializeSDataForMe()
        {
            //remember _service is global (defined up above)
            _service = new Sage.SData.Client.Core.SDataService();
            //yes the above line could have been shorter if I used a using statement;
            //Doing that makes the code harder to read IMO.

            // set user name to authenticate with
            _service.UserName = "admin";
            // set password to authenticate with
            _service.Password = "";

            //http://localhost:3333/sdata/slx/dynamic/-/clientprojects
            _service.Protocol = "HTTP";
            _service.ServerName = "localhost:3333";
            _service.ApplicationName = "slx";
            _service.VirtualDirectory = "sdata";
            _service.ContractName = "dynamic";
            _service.DataSet = "-";

            //another way of doing this: 
            //mydataService = new SDataService("http://localhost:2001/sdata/slx/dynamic/-/", "admin", "");
        }

        public Form1()
        {
            InitializeComponent();
            InitializeSDataForMe();
            LoadTickets();
        }

        public void LoadTickets()
        {
            Sage.SData.Client.Core.SDataSingleResourceRequest request = new 
                Sage.SData.Client.Core.SDataSingleResourceRequest(_service);


            request.ResourceKind = "tickets";
            //**Be sure to change this query!

            request.QueryValues.Add("include", "Account");
            request.ResourceSelector = "'tDEMOA000002'";
            // Read the feed from the server
            SDataPayload ticket, account;
            
            //I did this as a seperate so that I could tell when the read comes back
            Sage.SData.Client.Atom.AtomEntry entry = request.Read();
            //and clear the please wait message.
            //first get the payload out for the entry
            ticket = entry.GetSDataPayload();
            account = (SDataPayload)ticket.Values["Account"];
            account.Values["UserField1"] = "Sam";


            //put everything back..
            ticket.Values["Account"] = account;

            entry.SetSDataPayload(ticket);
            request.Entry = entry;
            request.Update();

            if (request.Entry.GetSDataHttpStatus() != 
                System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Uh oh. Something went wrong.");
            }           
        }
    }
}
