﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RdClient.Shared.Models;

namespace RdClient.Shared.Test.Helpers
{
    public class TestData
    {
        private Random _rand = new Random();

        public Random RandomSource
        {
            get
            {
                return _rand;
            }
        }

        public List<ModelBase> NewListOfModelBase()
        {            
            int count = RandomSource.Next(3, 10);
            List<ModelBase> result = new List<ModelBase>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(new ModelBase());
            }
            return result;
        }

        public string NewRandomString()
        {
            return "rand" + RandomSource.Next();
        }

        public Desktop NewValidDesktop(Guid credId)
        {
            return new Desktop() { HostName = NewRandomString(), CredentialId = credId };
        }

        public List<Desktop> NewSmallListOfDesktops(List<Credentials> creds)
        {
            int count = RandomSource.Next(3, 10);
            List<Desktop> desktops = new List<Desktop>(count);
            for (int i = 0; i < count; i++)
            {
                Guid credId = creds[_rand.Next(0, creds.Count)].Id;
                desktops.Add(NewValidDesktop(credId));
            }
            return desktops;
        }

        public Credentials NewValidCredential()
        {
            return new Credentials()
            {
                Domain = NewRandomString(),
                Username = NewRandomString(),
                Password = NewRandomString()
            };
        }

        public List<Credentials> NewSmallListOfCredentials()
        {
            int count = RandomSource.Next(3, 10);
            List<Credentials> creds = new List<Credentials>(count);
            for (int i = 0; i < count; i++)
            {
                creds.Add(NewValidCredential());
            }
            return creds;
        }
    }
}