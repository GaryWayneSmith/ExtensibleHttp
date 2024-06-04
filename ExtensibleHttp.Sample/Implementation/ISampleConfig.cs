namespace ExtensibleHttp.Sample.Implementation
{
	internal interface ISampleConfig
	{
		string Authorization { get; }
		string ChannelType { get; }
		string ClientId { get; }
		string ClientSecret { get; }
		string ServiceName { get; }

		Task ValidateToken(CancellationToken cancellationToken);
	}
}