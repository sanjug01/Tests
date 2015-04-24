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

        public IModelContainer<DesktopModel> NewValidDesktop(Guid credId)
        {            
            return TemporaryModelContainer<DesktopModel>.WrapModel(Guid.NewGuid(), new DesktopModel()
            {
                HostName = NewRandomString(),
                CredentialsId = credId
            });
        }

        public IModelContainer<DesktopModel> NewValidDesktopWithGateway(Guid gatewayId)
        {
            return TemporaryModelContainer<DesktopModel>.WrapModel(Guid.NewGuid(), new DesktopModel()
            {
                HostName = NewRandomString(),
                CredentialsId = Guid.Empty,
                GatewayId = gatewayId
            });
        }

        public IModelContainer<GatewayModel> NewValidGateway()
        {
            return TemporaryModelContainer<GatewayModel>.WrapModel(Guid.NewGuid(),
                new GatewayModel()
                {
                    HostName = NewRandomString(),
                    CredentialsId = Guid.Empty
                });
        }

        public IList<IModelContainer<GatewayModel>> NewSmallListOfGateways()
        {
            int count = RandomSource.Next(3, 10);
            IList<IModelContainer<GatewayModel>> gateways = new List<IModelContainer<GatewayModel>>(count);

            for (int i = 0; i < count; i++)
            {
                gateways.Add(NewValidGateway());
            }

            return gateways;
        }

        public GatewayModel NewValidGatewayWithCredential(Guid credId)
        {
            return new GatewayModel() { HostName = NewRandomString(), CredentialsId = credId };
        }

        public IList<IModelContainer<DesktopModel>> NewSmallListOfDesktops(IList<IModelContainer<CredentialsModel>> creds)
        {
            int count = RandomSource.Next(3, 10);
            IList<IModelContainer<DesktopModel>> desktops = new List<IModelContainer<DesktopModel>>(count);
            for (int i = 0; i < count; i++)
            {
                Guid credId = creds[_rand.Next(0, creds.Count)].Id;
                desktops.Add(NewValidDesktop(credId));
            }
            return desktops;
        }

        public IList<IModelContainer<DesktopModel>> NewSmallListOfDesktopsWithGateway(IList<IModelContainer<GatewayModel>> gateways)
        {
            int count = RandomSource.Next(3, 10);
            IList<IModelContainer<DesktopModel>> desktops = new List<IModelContainer<DesktopModel>>(count);
            for (int i = 0; i < count; i++)
            {
                Guid gatewayId = gateways[_rand.Next(0, gateways.Count)].Id;
                desktops.Add(NewValidDesktopWithGateway(gatewayId));
            }
            return desktops;
        }

        public IModelContainer<CredentialsModel> NewValidCredential()
        {
            return TemporaryModelContainer<CredentialsModel>.WrapModel(Guid.NewGuid(), new CredentialsModel()
            {
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

        public OnPremiseWorkspaceModel NewOnPremWorkspace(IRadcClient radcClient, ApplicationDataModel dataModel, Guid credId)
        {
            var workspace = new OnPremiseWorkspaceModel();
            workspace.Initialize(radcClient, dataModel);
            workspace.FeedUrl = "https://" + NewRandomString();
            workspace.CredentialsId = credId;
            return workspace;
        }

        public IList<OnPremiseWorkspaceModel> NewSmallListOfOnPremWorkspaces(IRadcClient radcClient, ApplicationDataModel dataModel, IList<IModelContainer<CredentialsModel>> creds)
        {
            int count = RandomSource.Next(3, 10);
            IList<OnPremiseWorkspaceModel> workspaces = new List<OnPremiseWorkspaceModel>(count);
            for (int i = 0; i < count; i++)
            {
                Guid credId = creds[_rand.Next(0, creds.Count)].Id;
                workspaces.Add(NewOnPremWorkspace(radcClient, dataModel, credId));
            }
            return workspaces;
        }
    }
}
