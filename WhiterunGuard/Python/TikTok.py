import asyncio

import httpx
from TikTokLive import TikTokLiveClient

client: TikTokLiveClient = TikTokLiveClient(
    unique_id="@lylaskyrim"
)



def check_live(client):
    try:
        loop = asyncio.get_event_loop()
        if loop.is_running():
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
    except RuntimeError: 
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)


    try:
        x = loop.run_until_complete(client.is_live())
        return x
    except httpx.ConnectTimeout:
        return False
