using InnerNet;

namespace HydraMenu.modules.troll
{
	internal class VotekickRetaliation : Module
	{
		public VotekickRetaliation() : base("VotekickRetaliation") { }

		private byte _triggerVoteCount = 0;

		// The amount of votes against us that will kick everyone out of the lobby
		// A value of zero disables the feature, and the highest useful value is one vote below the votekick threshold
		// as the server will have already kicked us out of the lobby once that threshold is reached
		public byte TriggerVoteCount
		{
			get { return _triggerVoteCount; }
			set
			{
				_triggerVoteCount = value;

				// The module only has to listen for votekicks while the feature is actually set to trigger at some point
				Enabled = value != 0;
			}
		}

		private void OnPlayerVotekick(ClientData source, ClientData target)
		{
			if(target.Id != AmongUsClient.Instance.ClientId) return;

			byte voteCount = Utilities.GetVotekickCount();
			if(voteCount < TriggerVoteCount) return;

			Hydra.notifications.Send("Votekick Retaliation", $"You have reached {voteCount} votekick(s), kicking everyone out of the lobby.", 5);

			Utilities.KickAllPlayers();
		}

		protected override void OnEnable()
		{
			EventCoordinator.OnPlayerVotekick += OnPlayerVotekick;
		}

		protected override void OnDisable()
		{
			EventCoordinator.OnPlayerVotekick -= OnPlayerVotekick;
		}
	}
}