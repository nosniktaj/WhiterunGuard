import asyncio

from TikTokLive import TikTokLiveClient

client: TikTokLiveClient = TikTokLiveClient(
    unique_id="@lylaskyrim"
)



def check_live(client):
    loop = asyncio.get_event_loop()
    x = loop.run_until_complete(client.is_live())
    return x

  
