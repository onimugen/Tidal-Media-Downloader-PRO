<div align="center">
  <h1>Tidal-Media-Downloader-PRO</h1>
  <a href="https://github.com/yaronzz/Tidal-Media-Downloader-PRO/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/yaronzz/Tidal-Media-Downloader-PRO.svg?style=flat-square" alt="">
  </a>
  <a href="https://github.com/yaronzz/Tidal-Media-Downloader-PRO/releases">
    <img src="https://img.shields.io/github/v/release/yaronzz/Tidal-Media-Downloader-PRO.svg?style=flat-square" alt="">
  </a>
  <a href="https://www.python.org/">
    <img src="https://img.shields.io/github/issues/yaronzz/Tidal-Media-Downloader-PRO.svg?style=flat-square" alt="">
  </a>
  <a href="https://github.com/yaronzz/Tidal-Media-Downloader-PRO/releases">
    <img src="https://img.shields.io/github/downloads/yaronzz/Tidal-Media-Downloader-PRO/total?label=tidal-gui%20download" alt="">
  </a>
  <a href="https://pypi.org/project/tidal-dl/">
    <img src="https://img.shields.io/pypi/dm/tidal-dl?label=tidal-dl%20download" alt="">
  </a>
</div>
<p align="center">
  «Tidal-Media-Downloader» is an application that lets you download videos and tracks from Tidal. It supports two version: tidal-dl and tidal-gui. 
    <br>
        <a href="https://github.com/yaronzz/Tidal-Media-Downloader-PRO/releases">Download</a> |
        <a href="https://yaronzz.com/post/tidal_dl_installation/">Documentation</a> |
        <a href="https://yaronzz.com/post/tidal_dl_installation_chn/">中文文档</a> |
        <a href="https://t.me/Tidal_Media_Downloader">Channel</a>
    <br>
</p>


## 📺 Installation
| Name           | platform                          | Install                                                      |
| -------------- | --------------------------------- | ------------------------------------------------------------ |
| tidal-gui      | Windows                           | [tidal-gui.exe](https://github.com/yaronzz/Tidal-Media-Downloader-PRO/releases) |
| tidal-dl (cli) | Windows \ Linux \ Macos \ Android | ```pip3 install tidal-dl --upgrade```<br />[Detailed Description](https://yaronzz.com/post/tidal_dl_installation/#Install) |

### Nightly Builds

|Download nightly builds from continuous integration: 	| [![Build Status][Build]][Actions] 
|-------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|

[Actions]: https://github.com/yaronzz/Tidal-Media-Downloader-PRO/actions/workflows/continuous-integration-workflow.yml
[Build]: https://github.com/yaronzz/Tidal-Media-Downloader-PRO/actions/workflows/continuous-integration-workflow.yml/badge.svg


## 📡 Telegram

- [Group](https://t.me/tidal_group) : Feed back
- [Channel](https://t.me/Tidal_Media_Downloader) : Notify the new version 

## 🤖 Features
- Download album \ track \ video \ playlist \ artist-albums
- Add metadata to songs
- Selectable video resolution and track quality
- **DASH streaming support** — Tidal now delivers all tracks as MPEG-DASH manifests; segments are downloaded and merged transparently via ffmpeg
- **Quality badge** — shows the actual quality received: `FLAC Max` (24-bit HiRes), `FLAC HiFi` (16-bit CD), `AAC 320`, `AAC 96`
- **Browser login** — one-click login via embedded browser; Bearer token is captured automatically, no manual copying needed

## 🔐 Login

Tidal's public API no longer grants device-code OAuth clients, so the classic "enter your code at tidal.com/link" flow does not work. Two alternatives are supported:

### Option 1 — Browser Login (recommended)
1. Click **Login with Browser (Auto)** on the LOGIN tab
2. An embedded browser opens — log in to your Tidal account normally
3. The token is captured automatically and the window closes

### Option 2 — Manual Token
1. Open [listen.tidal.com](https://listen.tidal.com) in any browser and log in
2. Press **F12 → Network** tab, click any request to `api.tidal.com`
3. Copy the value after `Authorization: Bearer ` in the request headers
4. Paste it in the **Token** field on the LOGIN tab and click **Login (Token)**

### KEYS tab (optional)
If you have your own Tidal API `ClientId` / `ClientSecret`, enter them in the **KEYS** tab. Otherwise the app fetches a working key automatically.

## 🎵 Audio Quality

The app requests the best quality your subscription supports and cascades down automatically:

| Badge | Quality | Format |
|-------|---------|--------|
| FLAC Max | HI_RES_LOSSLESS (up to 24-bit / 192kHz) | .flac |
| FLAC HiFi | LOSSLESS (16-bit / 44.1kHz CD) | .flac |
| AAC 320 | HIGH (320 kbps) | .m4a |
| AAC 96 | LOW (96 kbps) | .m4a |

> **Note:** HiRes Lossless requires a Tidal HiFi Plus subscription. MQA is no longer offered by Tidal.

## 💽 User Interface
<img src="https://i.loli.net/2020/08/19/gqW6zHI1SrKlomC.png" alt="image" style="zoom: 50%;" />

## ☕ Support

If you really like my projects and want to support me, you can buy me a coffee and star this project. 

<a href="https://www.buymeacoffee.com/yaronzz" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/arial-orange.png" alt="Buy Me A Coffee" style="height: 51px !important;width: 217px !important;" ></a>

## 🎂 Contributors
This project exists thanks to all the people who contribute. 

<a href="https://github.com/yaronzz/Tidal-Media-Downloader-PRO/graphs/contributors"><img src="https://contributors-img.web.app/image?repo=yaronzz/Tidal-Media-Downloader-PRO" /></a>


## 📜 Disclaimer
- Private use only.
- Need a Tidal-HIFI subscription. 
- You should not use this method to distribute or pirate music.
- It may be illegal to use this in your country, so be informed.


