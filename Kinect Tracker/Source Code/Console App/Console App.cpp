#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <Kinect.h>

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "kinect20.lib")

// Safe release for COM objects
template<class Interface>
inline void SafeRelease(Interface*& release)
{
    if (release != NULL)
    {
        release->Release();
        release = NULL;
    }
}

int main()
{
    // UDP Stuff
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);

    SOCKET outSock = socket(AF_INET, SOCK_DGRAM, 0);
    sockaddr_in serverHints;
    serverHints.sin_family = AF_INET;
    serverHints.sin_port = htons(5005); // Port for Unity
    inet_pton(AF_INET, "127.0.0.1", &serverHints.sin_addr);

    // Initialise the Kinect
    IKinectSensor* Sensor = NULL;
    IBodyFrameReader* BodyReader = NULL;

    HRESULT hr = GetDefaultKinectSensor(&Sensor);
    if (FAILED(hr)) {
        std::cout << "Failed to find the Kinect sensor [0]" << std::endl;
        return 1;
    }

    if (Sensor) {
        Sensor->Open();
        IBodyFrameSource* pBodySource = NULL;
        Sensor->get_BodyFrameSource(&pBodySource);

        if (pBodySource) {
            pBodySource->OpenReader(&BodyReader);
            SafeRelease(pBodySource);
        }
    }
    else {
        std::cout << "Failed to find the Kinect sensor [1]" << std::endl;
        return 1;
    }

    std::cout << "--- SENDING RAW HEAD DATA TO UNITY (5005) ---" << std::endl;

    // Main Loop
    while (true) {
        IBodyFrame* pBodyFrame = NULL;
        hr = BodyReader->AcquireLatestFrame(&pBodyFrame);

        if (SUCCEEDED(hr)) {
            IBody* ppBodies[BODY_COUNT] = { 0 };
            hr = pBodyFrame->GetAndRefreshBodyData(BODY_COUNT, ppBodies);

            if (SUCCEEDED(hr)) {
                for (int i = 0; i < BODY_COUNT; ++i) {
                    IBody* pBody = ppBodies[i];
                    if (pBody) {
                        BOOLEAN bTracked = false;
                        pBody->get_IsTracked(&bTracked);

                        if (bTracked) {
                            Joint joints[JointType_Count];
                            pBody->GetJoints(JointType_Count, joints);

                            // Get Raw Head Position
                            CameraSpacePoint rawPos = joints[JointType_Head].Position;

                            // --- SEND RAW DATA DIRECTLY ---
                            std::string data = std::to_string(rawPos.X) + "," +
                                std::to_string(rawPos.Y) + "," +
                                std::to_string(rawPos.Z);

                            sendto(outSock, data.c_str(), (int)data.size(), 0, (sockaddr*)&serverHints, sizeof(serverHints));

                            // Visual Feedback
                            printf("Raw Pos: X:%.3f Y:%.3f Z:%.3f      \r", rawPos.X, rawPos.Y, rawPos.Z);

                            // Break after finding the first tracked person
                            break;
                        }
                    }
                }
            }

            // Cleanup frame resources
            for (int i = 0; i < BODY_COUNT; ++i) SafeRelease(ppBodies[i]);
            SafeRelease(pBodyFrame);
        }
    }

    // --- 4. CLEANUP ---
    SafeRelease(BodyReader);
    if (Sensor) Sensor->Close();
    SafeRelease(Sensor);

    closesocket(outSock);
    WSACleanup();

    return 0;
}