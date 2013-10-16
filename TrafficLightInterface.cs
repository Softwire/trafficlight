namespace trafficlight
{
    internal class TrafficLightInterface : RelayBoardInterface
    {
        private void SetRYG(bool red, bool yellow, bool green)
        {
            // Due to the way the relays are wired, a "1" bit is off
            byte pins = 7;

            if (red)
            {
                pins ^= 4;
            }

            if (yellow)
            {
                pins ^= 2;
            }

            if (green)
            {
                pins ^= 1;
            }

            SetPins(pins);
        }

        public void SetRed()
        {
            SetRYG(true, false, false);
        }

        public void SetYellow()
        {
            SetRYG(false, true, false);
        }

        public void SetGreen()
        {
            SetRYG(false, false, true);
        }

        public void SetRedAndYellow()
        {
            SetRYG(true, true, false);
        }

        public void SetYellowAndGreen()
        {
            SetRYG(false, true, true);
        }

        public void SetAll()
        {
            SetRYG(true, true, true);
        }

        public void SetOff()
        {
            SetRYG(false, false, false);
        }
    }
}
