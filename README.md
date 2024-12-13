# GPTAvatar Multiplayer Extension: AI-Based Avatars for Collaborative Education

This repository hosts the implementation of the multiplayer-enhanced version of **GPTAvatar**, a platform for AI-powered virtual avatars designed to facilitate interactive and immersive educational experiences. This project is part of the bachelor's thesis *"Creating Powerful AI-Based Avatars for Education: Developing a Multiplayer for GPTAvatar"*.

## Overview

**GPTAvatar Multiplayer Extension** is an innovative platform that integrates large language models (LLMs), multiplayer capabilities, and customizable 3D avatars to revolutionize collaborative learning in virtual environments. With features like real-time interaction, personalized feedback, and user-driven customization, this project seeks to improve both the immersion and effectiveness of educational tools.

### Key Features
- **Multiplayer Mode**: Collaborative interaction with peers and the AI avatar in real-time.
- **Customizable Avatars**: Integration with [Ready Player Me](https://readyplayer.me/) for personalized user representation.
- **3D Classroom Environment**: Navigate, interact, and engage in a fully immersive educational setting.
- **AI-Powered Teacher**: Leverages LLMs (like ChatGPT) to simulate realistic educational interactions, moderate discussions, and provide feedback.
- **Real-Time Voice Chat**: High-quality, low-latency communication between participants using Photon Voice.
- **Dynamic Learning Scenarios**: Support for various educational tasks, such as simulated oral exams or group discussions.

## Technical Details

### Built With
- **Unity**: 3D development platform for creating the virtual environment.
- **Photon Fusion 2**: For robust networking and multiplayer synchronization.
- **Photon Voice**: Ensures seamless real-time voice communication.
- **Ready Player Me**: Enables avatar creation and customization.
- **C# Scripts**: Power the core functionalities such as avatar management, networking, and AI integration.

### Core Components
1. **Player Spawner**: Handles instantiation and synchronization of players in the multiplayer environment.
2. **AI Manager**: Interfaces with the LLM for text-to-speech, speech-to-text, and conversational capabilities.
3. **Network Architecture**: Ensures smooth communication and state synchronization among players.

### Project Structure
- **Assets**: Contains Unity assets, including prefabs, scripts, and scenes.
- **Scripts**: Core C# scripts for functionality such as player movement, avatar customization, and AI interaction.
- **Libraries**: Dependencies like Photon Fusion, Photon Voice, and Ready Player Me.

## Setup Instructions

### Prerequisites
- Unity 2022.3.4f1 or later
- Internet connection for LLM and Photon services

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/GPTAvatar-Multiplayer.git
   cd GPTAvatar-Multiplayer
   ```
2. Open the project in Unity.
3. Configure your Photon Fusion and Photon Voice settings.
4. Run the project from Unity Editor or build it for your target platform.

### Avatar Customization
1. Create your avatar using [Ready Player Me](https://readyplayer.me/).
2. Copy the avatar URL and paste it in the provided field on the start screen.

## User Study & Results
As part of this project, a user study was conducted to evaluate the system’s usability and effectiveness. Participants engaged in simulated oral English exams with the AI avatar acting as a teacher. Results highlighted the potential of GPTAvatar for:
- Improving English language skills.
- Reducing exam anxiety through realistic practice scenarios.
- Facilitating collaborative learning and interaction.

## Future Work
The following areas are proposed for future development:
- Enhanced avatar expressiveness (e.g., non-verbal cues and gestures).
- Integration with VR devices for immersive learning.
- Expansion to support additional subjects and skills.

## License
This project is licensed under the [Creative Commons Attribution-ShareAlike 4.0 License](http://creativecommons.org/licenses/by-sa/4.0/).

## Acknowledgments
- **Supervisor**: Dr. Maximilian C. Fink
- **Examiner**: Prof. Dr. Albrecht Schmidt
- Special thanks to the participants of the user study for their valuable feedback.
