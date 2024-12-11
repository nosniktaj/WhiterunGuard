import asyncio

from TikTokLive import TikTokLiveClient

client: TikTokLiveClient = TikTokLiveClient(
    unique_id="@lylaskyrim"
)



def check_live():
   x = asyncio.run(client.is_live())
   return x


  
