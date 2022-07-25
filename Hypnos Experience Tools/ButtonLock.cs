using System;
using System.Windows.Forms;

namespace Hypnos.Experience.Tools
{
	class ButtonLock : IDisposable
    {
        private Control _control = null;
        private bool _showWaitCursor = false;

        public ButtonLock(Control control, bool showWaitCursor)
        {
            _control = control;
            _showWaitCursor = showWaitCursor;

            if (_showWaitCursor)
                Cursor.Current = Cursors.WaitCursor;

            _control.Enabled = false;
            _control.Update();
        }

        public void Dispose()
        {
            Application.DoEvents();

            if (!_control.IsDisposed)
                _control.Enabled = true;

            if (_showWaitCursor)
                Cursor.Current = Cursors.Default;
        }
    }
}
