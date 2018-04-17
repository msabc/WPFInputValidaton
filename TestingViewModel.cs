using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class TestingViewModel : BaseViewModel
    {
        
        
        private string tekst;

        [Validated("You have to enter something.")]
        public string Tekst
        {
            get { return tekst; }
            set
            {
                SetValue(ref tekst, value);
            }
        }

        protected override void InitializeValidators()
        {
            AddValidator(nameof(Tekst), () => Tekst?.Length > 0);
        }
    }
}
