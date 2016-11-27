using System.Windows.Forms;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using UsersAndRolesWF;
using SharedModels;

namespace UsersAndRolesWF
{
    public partial class Frm_Login : Form
    {
        public Frm_Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            if (VerifyUserNamePassword(txtUsername.Text, txtPassword.Text))
            {
                Frm_Main.auth = true;
                Close();
            }
            else
                MessageBox.Show("Wrong username or password.");
        }
        public bool VerifyUserNamePassword(string userName, string password)
        {
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = usermanager.Find(userName, password);
            if (user != null)
            {
                if (usermanager.IsInRole(user.Id, "Admin"))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
