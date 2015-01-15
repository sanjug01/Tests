namespace RdClient.Shared.Input
{
    using System;
    using Windows.System;
    using Windows.UI.Core;

    public sealed class CharacterKey : VirtualKeyBase
    {
        private sealed class PressedCharacter : IPhysicalKeyData
        {
            private readonly char _character;

            public PressedCharacter(char character)
            {
                _character = character;
            }

            public char Character { get { return _character; } }
        }

        public CharacterKey(VirtualKey virtualKey, IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
            : base(virtualKey, keyboardSink, keyboardState)
        {
        }

        protected override bool OnPressed(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            this.RegisterPhysicalKey(keyStatus);
            return false;
        }

        protected override void OnReleased(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            dataContainer.DoIf<PressedCharacter>(pc =>
            {
                this.ReportKeyEvent((int)pc.Character, false, keyStatus.IsExtendedKey, true);
            });

            this.UnregisterPhysicalKey(keyStatus);
        }

        protected override void OnCharacter(char character, CoreAcceleratorKeyEventType eventType, CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            dataContainer.KeyData = new PressedCharacter(character);
            this.ReportKeyEvent((int)character, false, keyStatus.IsExtendedKey, false);
        }
    }
}
