using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DPA_Musicsheets.State
{
    public class TextEditedState : TextEditState
    {
        public TextEditedState(ICanHaveTextEditState context) : base(context)
        {

        }

        public override void Exit()
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Don't forget to save your work!", button);

            switch(result)
            {
                case MessageBoxResult.Yes:

                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
