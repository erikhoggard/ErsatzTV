# ErsatzTV (Custom Fork: Show Probabilities)

This fork includes a custom feature: **Program Schedule Show Probabilities**.

### Feature Description: Show Probabilities
This feature changes how ErsatzTV selects the next item to play by splitting the decision into two distinct steps:
1.  **Weighted Show Selection:** First, it uses your configured probability sliders to randomly select a **Show**.
2.  **Episode Selection:** Then, it picks an **Episode** from that specific Show based on the Schedule's Playback Order (Random, Sequential, etc.).

### Detailed Behavior by Mode
When "Use Custom Show Probabilities" is enabled, it overrides the standard "deck" behavior for the following modes:

*   **Shuffle:**
    *   **Behavior:** It randomly picks a Show (based on weight), then plays a random episode from that show.
    *   **Looping:** Shows cycle continuously. When all episodes of a show have played, it reshuffles and starts over while maintaining the configured weight distribution.
    *   *Result:* Show A (high weight) and Show B (low weight) will interleave based on their weights indefinitely.

*   **Shuffle In Order:**
    *   **Behavior:** It randomly picks a Show (based on weight), then plays the **next sequential** episode (S01E01, then S01E02...).
    *   *Result:* You get a "channel surfing" experience where you mostly see Show A, but occasionally see Show B, and both progress in order.

*   **Chronological / SeasonEpisode:**
    *   **Behavior:** Identical to *Shuffle In Order* above.
    *   *Note:* Standard Chronological behavior usually plays *all* of Show A, then *all* of Show B. Enabling probabilities effectively transforms this into an "Interleaved Sequential" mode.

*   **Random:**
    *   **Behavior:** It randomly picks a Show (based on weight), then plays a random episode.
    *   **Looping:** Pure weighted random selection - the same episode may repeat before all episodes have played.

### "Shows" vs "Other Content"
*   **TV Shows:** The configuration menu scans your schedule and creates a slider for every specific TV Show found.
*   **Movies / Music Videos:** The current implementation groups all non-episode content (Movies, Music Videos, etc.) into a single invisible "Other" bucket. This bucket has a default weight of `1` and cannot currently be adjusted in the UI.

### Summary of What You Achieved
You have successfully decoupled "Probability of Appearance" from "Number of Episodes".
*   **Before:** A show with 900 episodes was 9x more likely to appear than a show with 100 episodes.
*   **Now:** You can set the 100-episode show to have a weight of `50` and the 900-episode show to `10`. The short show will now appear 5x more often than the long show, cycling through its episodes repeatedly to maintain the configured distribution.

---

# ErsatzTV

ErsatzTV lets you transform your media library into a personalized, live TV experience - complete with EPG, channel scheduling, and seamless streaming to all your devices. Rediscover your content, your way.

[![discord](https://img.shields.io/badge/join_discord-510b80?style=for-the-badge&logo=discord)](https://discord.ersatztv.org)
[![roadmap](https://img.shields.io/badge/vote_on_features-darkgreen?style=for-the-badge)](https://features.ersatztv.org/)
[![community](https://img.shields.io/badge/join_the_community-blue?style=for-the-badge)](https://discuss.ersatztv.org)

![epg-example](https://ersatztv.org/images/home/epg-example.png)

## How It Works

1. **Install ErsatzTV**: Download and set up the server on your system.
2. **Add Your Media**: Connect your media libraries and collections.
3. **Create Channels**: Design and schedule your own live channels.
4. **Stream Anywhere**: Watch on any device with IPTV and EPG support.

## Key Features

- **Custom channels**: Create and schedule your own live TV channels.
- **IPTV & EPG**: Stream with IPTV and Electronic Program Guide support.
- **Hardware Transcoding**: High-performance streaming with hardware acceleration (NVENC, QSV, VAAPI, AMF, VideoToolbox)
- **Media Server Integration**: Connect Plex, Jellyfin, Emby and more.
- **Music & Subtitles**: Mix music videos and enjoy subtitle support.
- **Open Source**: Free, open, and community-driven project.

## Documentation

Documentation is available at [ersatztv.org](https://ersatztv.org/docs/).

## License

This project is inspired by [pseudotv-plex](https://github.com/DEFENDORe/pseudotv) and
the [dizquetv](https://github.com/vexorian/dizquetv) fork and is released under the [zlib license](LICENSE).
