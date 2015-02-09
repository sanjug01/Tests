using RdClient.Shared.CxWrappers;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Helpers
{
    public class TestData
    {
        private readonly Random _rand = new Random(42);

        public Random RandomSource
        {
            get
            {
                return _rand;
            }
        }

        public string NewRandomString()
        {
            return "rand" + RandomSource.Next();
        }

        public RdpScreenSnapshot NewValidScreenSnapshot(int width, int height)
        {
            byte[] bytes = new byte[width * height * 4];
            this.RandomSource.NextBytes(bytes);
            return new RdpScreenSnapshot(width, height, bytes);
        }

        public DesktopModel NewValidDesktop(Guid credId)
        {
            return new DesktopModel() { HostName = NewRandomString(), CredentialsId = credId };
        }

        public IList<DesktopModel> NewSmallListOfDesktops(IList<IModelContainer<CredentialsModel>> creds)
        {
            int count = RandomSource.Next(3, 10);
            IList<DesktopModel> desktops = new List<DesktopModel>(count);
            for (int i = 0; i < count; i++)
            {
                Guid credId = creds[_rand.Next(0, creds.Count)].Id;
                desktops.Add(NewValidDesktop(credId));
            }
            return desktops;
        }

        public IModelContainer<CredentialsModel> NewValidCredential()
        {
            return TemporaryModelContainer<CredentialsModel>.WrapModel(Guid.NewGuid(), new CredentialsModel()
            {
                Domain = NewRandomString(),
                Username = NewRandomString(),
                Password = NewRandomString()
            });
        }

        public IList<IModelContainer<CredentialsModel>> NewSmallListOfCredentials()
        {
            int count = RandomSource.Next(3, 10);
            IList<IModelContainer<CredentialsModel>> creds = new List<IModelContainer<CredentialsModel>>(count);

            for (int i = 0; i < count; i++)
            {
                creds.Add(NewValidCredential());
            }

            return creds;
        }
    }
}
