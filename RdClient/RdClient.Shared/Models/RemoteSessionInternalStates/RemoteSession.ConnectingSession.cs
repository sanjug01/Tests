namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Telemetry;
    using RdClient.Shared.ViewModels;
    using RdClient.Shared.ViewModels.EditCredentialsTasks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading;
    using Windows.Networking.Connectivity;

    partial class RemoteSession
    {
        private sealed class ConnectingSession : InternalState
        {
            private readonly IRenderingPanel _renderingPanel;
            private IRdpConnection _connection;
            private bool _cancelledCredentials;

            public ConnectingSession(IRenderingPanel renderingPanel, InternalState otherState)
                : base(SessionState.Connecting, otherState)
            {
                Contract.Assert(null != renderingPanel);

                _renderingPanel = renderingPanel;
                _cancelledCredentials = false;
            }

            public ConnectingSession(IRenderingPanel renderingPanel, IRdpConnection connection, InternalState otherState)
                : base(SessionState.Connecting, otherState)
            {
                Contract.Assert(null != renderingPanel);
                Contract.Assert(null != connection);

                _renderingPanel = renderingPanel;
                _connection = connection;
                _cancelledCredentials = false;
            }

            protected override void Activated()
            {
                _renderingPanel.Ready += this.OnRenderingPanelReady;

                if (!this.Session._networkTypeReported)
                {
                    CollectNetworkTelemetry();
                    this.Session._networkTypeReported = true;
                }
            }

            protected override void Terminate()
            {
                ITelemetryEvent te = this.MakeTelemetryEvent("UserAction");
                te.AddTag("action", "Cancel");
                te.Report();

                if (null != _connection)
                    _connection.Disconnect();
            }

            protected override void Completed()
            {
                _renderingPanel.Ready -= this.OnRenderingPanelReady;

                if (null != _connection)
                {
                    this.Session._syncEvents.ClientConnected -= this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect -= this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected -= this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived -= this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust -= this.OnCheckGatewayCertificateTrust;
                }
            }

            private void OnRenderingPanelReady(object sender, EventArgs e)
            {
                if (null == _connection)
                {
                    _connection = this.Session.InternalCreateConnection(_renderingPanel);
                    Contract.Assert(null != _connection);
                    Contract.Assert(null != this.Session._syncEvents);

                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived += this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust += this.OnCheckGatewayCertificateTrust;

                    _connection.SetCredentials(this.Session._sessionSetup.SessionCredentials.Credentials,
                        !this.Session._sessionSetup.SessionCredentials.IsNewPassword);

                    // pass gateway, if necessary
                    if (this.Session._sessionSetup.SessionGateway.HasGateway)
                    {
                        _connection.SetGateway(
                            this.Session._sessionSetup.SessionGateway.Gateway,
                            this.Session._sessionSetup.SessionGateway.Credentials);
                    }

                    _connection.Connect();
                }
                else
                {
                    this.Session._syncEvents.ClientConnected += this.OnClientConnected;
                    this.Session._syncEvents.ClientAsyncDisconnect += this.OnClientAsyncDisconnect;
                    this.Session._syncEvents.ClientDisconnected += this.OnClientDisconnected;
                    this.Session._syncEvents.StatusInfoReceived += this.OnStatusInfoReceived;
                    this.Session._syncEvents.CheckGatewayCertificateTrust += this.OnCheckGatewayCertificateTrust;
                }
            }

            private enum IanaInterfaceType : uint
            {
                regular1822 = 2,
                hdh1822 = 3,
                ddnX25 = 4,
                rfc877x25 = 5,
                ethernet = 6,
                iso88023Csmacd = 7,
                iso88024TokenBus = 8,
                iso88025TokenRing = 9,
                iso88026Man = 10,
                starLan = 11,
                proteon10Mbit = 12,
                proteon80Mbit = 13,
                hyperchannel = 14,
                fddi = 15,
                lapb = 16,
                sdlc = 17,
                ds1 = 18,
                e1 = 19,
                basicISDN = 20,
                primaryISDN = 21,
                propPointToPointSerial = 22,
                ppp = 23,
                softwareLoopback = 24,
                eon = 25,
                ethernet3Mbit = 26,
                nsip = 27,
                slip = 28,
                ultra = 29,
                ds3 = 30,
                sip = 31,
                frameRelay = 32,
                rs232 = 33,
                para = 34,
                arcnet = 35,
                arcnetPlus = 36,
                atm = 37,
                miox25 = 38,
                sonet = 39,
                x25ple = 40,
                iso88022llc = 41,
                localTalk = 42,
                smdsDxi = 43,
                frameRelayService = 44,
                v35 = 45,
                hssi = 46,
                hippi = 47,
                modem = 48,
                aal5 = 49,
                sonetPath = 50,
                sonetVT = 51,
                smdsIcip = 52,
                propVirtual = 53,
                propMultiplexor = 54,
                ieee80212 = 55,
                fibreChannel = 56,
                hippiInterface = 57,
                frameRelayInterconnect = 58,
                aflane8023 = 59,
                aflane8025 = 60,
                cctEmul = 61,
                fastEther = 62,
                isdn = 63,
                v11 = 64,
                v36 = 65,
                g703at64k = 66,
                g703at2mb = 67,
                qllc = 68,
                fastEtherFX = 69,
                channel = 70,
                ieee80211 = 71,
                ibm370parChan = 72,
                escon = 73,
                dlsw = 74,
                isdns = 75,
                isdnu = 76,
                lapd = 77,
                ipSwitch = 78,
                rsrb = 79,
                atmLogical = 80,
                ds0 = 81,
                ds0Bundle = 82,
                bsc = 83,
                async = 84,
                cnr = 85,
                iso88025Dtr = 86,
                eplrs = 87,
                arap = 88,
                propCnls = 89,
                hostPad = 90,
                termPad = 91,
                frameRelayMPI = 92,
                x213 = 93,
                adsl = 94,
                radsl = 95,
                sdsl = 96,
                vdsl = 97,
                iso88025CRFPInt = 98,
                myrinet = 99,
                voiceEM = 100,
                voiceFXO = 101,
                voiceFXS = 102,
                voiceEncap = 103,
                voiceOverIp = 104,
                atmDxi = 105,
                atmFuni = 106,
                atmIma = 107,
                pppMultilinkBundle = 108,
                ipOverCdlc = 109,
                ipOverClaw = 110,
                stackToStack = 111,
                virtualIpAddress = 112,
                mpc = 113,
                ipOverAtm = 114,
                iso88025Fiber = 115,
                tdlc = 116,
                gigabitEthernet = 117,
                hdlc = 118,
                lapf = 119,
                v37 = 120,
                x25mlp = 121,
                x25huntGroup = 122,
                transpHdlc = 123,
                interleave = 124,
                fast = 125,
                ip = 126,
                docsCableMaclayer = 127,
                docsCableDownstream = 128,
                docsCableUpstream = 129,
                a12MppSwitch = 130,
                tunnel = 131,
                coffee = 132,
                ces = 133,
                atmSubInterface = 134,
                l2vlan = 135,
                l3ipvlan = 136,
                l3ipxvlan = 137,
                digitalPowerline = 138, // IP over Power Lines
                mediaMailOverIp = 139, // Multimedia Mail over IP
                dtm = 140,        // Dynamic syncronous Transfer Mode
                dcn = 141,    // Data Communications Network
                ipForward = 142,    // IP Forwarding Interface
                msdsl = 143,       // Multi-rate Symmetric DSL
                ieee1394 = 144, // IEEE1394 High Performance Serial Bus
                if_gsn = 145,       //   HIPPI-6400 
                dvbRccMacLayer = 146, // DVB-RCC MAC Layer
                dvbRccDownstream = 147,  // DVB-RCC Downstream Channel
                dvbRccUpstream = 148,  // DVB-RCC Upstream Channel
                atmVirtual = 149,   // ATM Virtual Interface
                mplsTunnel = 150,   // MPLS Tunnel Virtual Interface
                srp = 151,  // Spatial Reuse Protocol
                voiceOverAtm = 152,  // Voice Over ATM
                voiceOverFrameRelay = 153,   // Voice Over Frame Relay
                idsl = 154,     // Digital Subscriber Loop over ISDN
                compositeLink = 155,  // Avici Composite Link Interface
                ss7SigLink = 156,     // SS7 Signaling Link
                propWirelessP2P = 157,  //  Prop.P2P wireless interface
                frForward = 158,    // Frame Forward Interface
                rfc1483 = 159,  // Multiprotocol over ATM AAL5
                usb = 160,      // USB Interface
                ieee8023adLag = 161,  // IEEE 802.3ad Link Aggregate
                bgppolicyaccounting = 162, // BGP Policy Accounting
                frf16MfrBundle = 163, // FRF .16 Multilink Frame Relay
                h323Gatekeeper = 164, // H323 Gatekeeper
                h323Proxy = 165, // H323 Voice and Video Proxy
                mpls = 166, // MPLS
                mfSigLink = 167, // Multi-frequency signaling link
                hdsl2 = 168, // High Bit-Rate DSL - 2nd generation
                shdsl = 169, // Multirate HDSL2
                ds1FDL = 170, // Facility Data Link 4Kbps on a DS1
                pos = 171, // Packet over SONET/SDH Interface
                dvbAsiIn = 172, // DVB-ASI Input
                dvbAsiOut = 173, // DVB-ASI Output
                plc = 174, // Power Line Communtications
                nfas = 175, // Non Facility Associated Signaling
                tr008 = 176, // TR008
                gr303RDT = 177, // Remote Digital Terminal
                gr303IDT = 178, // Integrated Digital Terminal
                isup = 179, // ISUP
                propDocsWirelessMaclayer = 180, // Cisco proprietary Maclayer
                propDocsWirelessDownstream = 181, // Cisco proprietary Downstream
                propDocsWirelessUpstream = 182, // Cisco proprietary Upstream
                hiperlan2 = 183, // HIPERLAN Type 2 Radio Interface
                propBWAp2Mp = 184, // PropBroadbandWirelessAccesspt2multipt
                                   // use of this iftype for IEEE 802.16 WMAN
                                   // interfaces as per IEEE Std 802.16f is
                                   // deprecated and ifType 237 should be used instead.
                sonetOverheadChannel = 185, // SONET Overhead Channel
                digitalWrapperOverheadChannel = 186, // Digital Wrapper
                aal2 = 187, // ATM adaptation layer 2
                radioMAC = 188, // MAC layer over radio links
                atmRadio = 189, // ATM over radio links
                imt = 190, // Inter Machine Trunks
                mvl = 191, // Multiple Virtual Lines DSL
                reachDSL = 192, // Long Reach DSL
                frDlciEndPt = 193, // Frame Relay DLCI End Point
                atmVciEndPt = 194, // ATM VCI End Point
                opticalChannel = 195, // Optical Channel
                opticalTransport = 196, // Optical Transport
                propAtm = 197, //  Proprietary ATM
                voiceOverCable = 198, // Voice Over Cable Interface
                infiniband = 199, // Infiniband
                teLink = 200, // TE Link
                q2931 = 201, // Q.2931
                virtualTg = 202, // Virtual Trunk Group
                sipTg = 203, // SIP Trunk Group
                sipSig = 204, // SIP Signaling
                docsCableUpstreamChannel = 205, // CATV Upstream Channel
                econet = 206, // Acorn Econet
                pon155 = 207, // FSAN 155Mb Symetrical PON interface
                pon622 = 208, // FSAN622Mb Symetrical PON interface
                bridge = 209, // Transparent bridge interface
                linegroup = 210, // Interface common to multiple lines
                voiceEMFGD = 211, // voice E&M Feature Group D
                voiceFGDEANA = 212, // voice FGD Exchange Access North American
                voiceDID = 213,
                mpegTransport = 214,
                sixToFour = 215,
                gtp = 216,
                pdnEtherLoop1 = 217,
                pdnEtherLoop2 = 218,
                opticalChannelGroup = 219,
                homepna = 220,
                gfp = 221,
                ciscoISLvlan = 222,
                actelisMetaLOOP = 223,
                fcipLink = 224,
                rpr = 225, // Resilient Packet Ring Interface Type
                qam = 226, // RF Qam Interface
                lmp = 227, // Link Management Protocol
                cblVectaStar = 228, // Cambridge Broadband Networks Limited VectaStar
                docsCableMCmtsDownstream = 229, // CATV Modular CMTS Downstream Interface
                adsl2 = 230, // Asymmetric Digital Subscriber Loop Version 2 
                             //  = DEPRECATED/OBSOLETED - please use adsl2plus 238 instead)
                macSecControlledIF = 231, // MACSecControlled
                macSecUncontrolledIF = 232, // MACSecUncontrolled
                aviciOpticalEther = 233, // Avici Optical Ethernet Aggregate
                atmbond = 234, // atmbond
                voiceFGDOS = 235, // voice FGD Operator Services
                mocaVersion1 = 236, // MultiMedia over Coax Alliance = MoCA) Interface
                                    // as documented in information provided privately to IANA
                ieee80216WMAN = 237, // IEEE 802.16 WMAN interface
                adsl2plus = 238, // Asymmetric Digital Subscriber Loop Version 2, 
                                 // Version 2 Plus and all variants
                dvbRcsMacLayer = 239, // DVB-RCS MAC Layer
                dvbTdm = 240, // DVB Satellite TDM
                dvbRcsTdma = 241, // DVB-RCS TDMA
                x86Laps = 242,
                wwanPP = 243,
                wwanPP2 = 244,
                voiceEBS = 245,
                ifPwType = 246,
                ilan = 247,
                pip = 248,
                aluELP = 249,
                gpon = 250,
                vdsl2 = 251,
                capwapDot11Profile = 252,
                capwapDot11Bss = 253,
                capwapWtpVirtualRadio = 254,
                bits = 255,
                docsCableUpstreamRfPort = 256,
                cableDownstreamRfPort = 257,
                vmwareVirtualNic = 258,
                ieee802154 = 259,
                otnOdu = 260,
                otnOtu = 261,
                ifVfiType = 262,
                g9981 = 263,
                g9982 = 264,
                g9983 = 265,
                aluEpon = 266,
                aluEponOnu = 267,
                aluEponPhysicalUni = 268,
                aluEponLogicalLink = 269,
                aluGponOnu = 270,
                aluGponPhysicalUni = 271,
                vmwareNicTeam = 272,
                docsOfdmDownstream = 277,
                docsOfdmaUpstream = 278,
                gfast = 279,
                sdci = 280
            }

            private static string GetIanaInterfaceTypeString(uint ianaType)
            {
                return ((IanaInterfaceType)ianaType).ToString();
            }

            private void OnCheckGatewayCertificateTrust(object sender, CheckGatewayCertificateTrustArgs e)
            {
                // (pre)validation of the gateway certificate, per RDPConnection request
                IRdpCertificate certificate = e.Certificate;
                Contract.Assert(null != certificate);
                
                if (this.Session._certificateTrust.IsCertificateTrusted(certificate)
                    || this.Session._sessionSetup.DataModel.CertificateTrust.IsCertificateTrusted(certificate))
                {
                    // The certificate has been accepted already;
                    e.TrustDelegate.Invoke(true);
                }
                else
                {
                    // The certificate has not been trusted yet
                    e.TrustDelegate.Invoke(false);
                }
            }

            private void OnClientConnected(object sender, ClientConnectedArgs e)
            {
                using(LockWrite())
                {
                    //
                    // Set the internal sesion state to Connected.
                    //
                    ChangeState(new ConnectedSession(_connection, this));
                }
            }

            private void OnClientAsyncDisconnect(object sender, ClientAsyncDisconnectArgs e)
            {
                Contract.Assert(sender is IRdpConnection);

                IRdpConnection connection = (IRdpConnection)sender;
                Contract.Assert(object.ReferenceEquals(connection, _connection));

                switch (e.DisconnectReason.Code)
                {
                    case RdpDisconnectCode.CertValidationFailed:
                        //
                        // Set the internal state to "certificate validation needed"
                        //                        
                        ValidateCertificate(connection.GetServerCertificate(), e.DisconnectReason, this.Session._sessionSetup.HostName);
                        break;

                    case RdpDisconnectCode.PreAuthLogonFailed:
                        RequestValidCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.FreshCredsRequired:
                        RequestNewPassword(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyNeedCredentials:
                        // Gateway needs credentials
                        RequestNewGatewayCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyLogonFailed:
                        // Gateway credentials failed - prompt for new credentials
                        RequestValidGatewayCredentials(e.DisconnectReason);
                        break;

                    case RdpDisconnectCode.ProxyInvalidCA:
                        // Gateway certificate needs validation
                        ValidateCertificate(connection.GetGatewayCertificate(), e.DisconnectReason,
                            this.Session._sessionSetup.SessionGateway.Gateway.HostName);
                        break;

                    case RdpDisconnectCode.CredSSPUnsupported:
                        // Set the internal state to "certificate validation needed"
                        // Should prompt that the server identity cannot be verified 
                        ValidateServerIdentity(this.Session._sessionSetup.HostName , e.DisconnectReason);
                        break;
                    default:
                        connection.HandleAsyncDisconnectResult(e.DisconnectReason, false);
                        break;
                }
            }
            private void OnClientDisconnected(object sender, ClientDisconnectedArgs e)
            {
                Contract.Assert(sender is IRdpConnection);
                Contract.Assert(object.ReferenceEquals(sender, _connection));
                Contract.Ensures(null == _connection);

                if (RdpDisconnectCode.UserInitiated == e.DisconnectReason.Code || _cancelledCredentials)
                {
                    //
                    // If user has disconnected (unlikely), or the credentials prompt was cancelled,
                    // go to the Closed state, so the session view will navigate to the connection center page.
                    //
                    ChangeState(new ClosedSession(_connection, this));
                }
                else
                {
                    //
                    // For all other failures go to the Failed state so the sessoin view will show the error UI.
                    //
                    ChangeState(new FailedSession(_connection, e.DisconnectReason, this));
                }
            }

            private void OnStatusInfoReceived(object sender, StatusInfoReceivedArgs e)
            {
                Debug.WriteLine("Connecting|StatusInfoReceived|StatusCode={0}", e.StatusCode);
            }

            private void ValidateCertificate(IRdpCertificate certificate, RdpDisconnectReason reason, string serverName)
            {
                Contract.Assert(null != certificate);
                Contract.Assert(null != _connection);

                if (this.Session._certificateTrust.IsCertificateTrusted(certificate)
                    || this.Session._sessionSetup.DataModel.CertificateTrust.IsCertificateTrusted(certificate))
                {
                    //
                    // The certificate has been accepted already;
                    //
                    _connection.HandleAsyncDisconnectResult(reason, true);
                }
                else
                {
                    //
                    // Set the state to ValidateCertificate, that will emit a BadCertificate event from the session
                    // and handle the user's response to the event.
                    //
                    ChangeState(new ValidateCertificate(_renderingPanel, _connection, reason, serverName, this));
                }
            }

            private void ValidateServerIdentity(String hostName, RdpDisconnectReason reason)
            {
                Contract.Assert(null != this.Session._sessionSetup);
                Contract.Assert(null != _connection);

                if(
                    this.Session._isServerTrusted || 
                    ( null != (this.Session._sessionSetup.Connection as DesktopModel)
                    && (this.Session._sessionSetup.Connection as DesktopModel).IsTrusted )
                  )
                {
                    _connection.HandleAsyncDisconnectResult(reason, true);
                }
                else
                {
                    ChangeState(new ValidateServerIdentity(_connection, reason, this));
                }
            }

            private void RequestValidCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(this.Session._sessionSetup.SessionCredentials,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.InvalidCredentials,
                    reason);

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewPassword(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(this.Session._sessionSetup.SessionCredentials,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.FreshCredentialsNeeded,
                    reason);

                task.Submitted += this.NewPasswordSubmitted;
                task.Cancelled += this.NewPasswordCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void NewPasswordSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;

                if (e.SaveCredentials)
                    this.Session._sessionSetup.SaveCredentials();
                //
                // Go ahead and try to re-connect with new credentials.
                // Stay in the same state, update the session credentials and call HandleAsyncDisconnectResult
                // to re-connect.
                //
                using (LockWrite())
                {
                    _connection.SetCredentials(this.Session._sessionSetup.SessionCredentials.Credentials,
                        !this.Session._sessionSetup.SessionCredentials.IsNewPassword);
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, true);
                }
            }

            private void NewPasswordCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
                ITelemetryEvent te = this.MakeTelemetryEvent("UserAction");
                te.AddTag("action", "CancelCredentials");
                te.Report();

                task.Submitted -= this.NewPasswordSubmitted;
                task.Cancelled -= this.NewPasswordCancelled;
                //
                // User has cancelled the credentials dialog.
                // Stay in the state and wait for the connection to terminate.
                //
                using(LockWrite())
                {
                    _cancelledCredentials = true;
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, false);
                }
            }


            private void RequestValidGatewayCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(
                    this.Session._sessionSetup.SessionGateway,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.InvalidGatewayCredentials,
                    reason);

                task.Submitted += this.NewGatewayCredentialsSubmitted;
                task.Cancelled += this.NewGatewayCredentialsCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void RequestNewGatewayCredentials(RdpDisconnectReason reason)
            {
                //
                // Emit an event with a credentials editor task.
                //
                InSessionCredentialsTask task = new InSessionCredentialsTask(
                    this.Session._sessionSetup.SessionGateway,
                    this.Session._sessionSetup.DataModel,
                    CredentialPromptMode.FreshGatewayCredentialsNeeded,
                    reason);

                task.Submitted += this.NewGatewayCredentialsSubmitted;
                task.Cancelled += this.NewGatewayCredentialsCancelled;

                this.Session.DeferEmitCredentialsNeeded(task);
            }

            private void NewGatewayCredentialsSubmitted(object sender, InSessionCredentialsTask.SubmittedEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;

                task.Submitted -= this.NewGatewayCredentialsSubmitted;
                task.Cancelled -= this.NewGatewayCredentialsCancelled;

                if (e.SaveCredentials)
                    this.Session._sessionSetup.SaveGatewayCredentials();
                //
                // Go ahead and try to re-connect with new gateway credentials.
                // Stay in the same state, update the session credentials and call HandleAsyncDisconnectResult
                // to re-connect.
                //
                using (LockWrite())
                {
                    _connection.SetGateway(this.Session._sessionSetup.SessionGateway.Gateway,
                        this.Session._sessionSetup.SessionGateway.Credentials);
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, true);
                }
            }

            private void NewGatewayCredentialsCancelled(object sender, InSessionCredentialsTask.ResultEventArgs e)
            {
                InSessionCredentialsTask task = (InSessionCredentialsTask)sender;
                ITelemetryEvent te = this.MakeTelemetryEvent("UserAction");
                te.AddTag("action", "CancelGatewayCredentials");
                te.Report();

                task.Submitted -= this.NewGatewayCredentialsCancelled;
                task.Cancelled -= this.NewGatewayCredentialsCancelled;
                //
                // User has cancelled the credentials dialog.
                // Stay in the state and wait for the connection to terminate.
                //
                using (LockWrite())
                {
                    _cancelledCredentials = true;
                    _connection.HandleAsyncDisconnectResult((RdpDisconnectReason)e.State, false);
                }
            }

            private void CollectNetworkTelemetry()
            {
                //
                // Collect network information and cram it into
                //
                ConnectionProfileFilter filter = new ConnectionProfileFilter() { IsConnected = true };
                IReadOnlyList<ConnectionProfile> profiles = NetworkInformation.FindConnectionProfilesAsync(filter).AsTask().Result;
                StringBuilder sb = null;

                foreach (ConnectionProfile profile in profiles)
                {
                    if (null == sb)
                        sb = new StringBuilder();
                    else
                        sb.Append(',');
                    sb.Append(GetIanaInterfaceTypeString(profile.NetworkAdapter.IanaInterfaceType));

                    if (profile.IsWwanConnectionProfile)
                    {
                        WwanDataClass dataClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();

                        foreach (WwanDataClass wwdc in Enum.GetValues(typeof(WwanDataClass)))
                        {
                            if (WwanDataClass.None != wwdc && wwdc == (dataClass & wwdc))
                            {
                                sb.Append(':');
                                sb.Append(wwdc);
                            }
                        }
                    }
                }

                this.SessionTelemetry.AddTag("networkType", null != sb ? sb.ToString() : "unknown");
            }
        }
    }
}
