using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace ApomContaEnviada
{
    [RunInstaller(true)]
    public partial class SrvTransEnviadaContaInstall : System.Configuration.Install.Installer
    {
        public SrvTransEnviadaContaInstall()
        {
            InitializeComponent();
        }
    }
}
