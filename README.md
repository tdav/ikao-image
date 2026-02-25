# OFIQ C# & ASP.NET Core Project

This project provides a comprehensive C# (.NET 10) wrapper and REST API for the **OFIQ (Open Source Face Image Quality)** library. It is designed to run on **Linux (x64)** and offers tools for facial image quality assessment according to international standards (ISO/IEC 29794-5).

This project is based on the official **BSI-OFIQ** implementation:  
[https://github.com/BSI-OFIQ/OFIQ-Project](https://github.com/BSI-OFIQ/OFIQ-Project)

---

## Project Components

The project consists of three main parts:

1.  **C++ Bridge (`ofiq_bridge.cpp`)**: A native wrapper that exposes the complex C++ OFIQ API as a C-compatible interface, enabling seamless integration with C# via P/Invoke.
2.  **Console Application (`ikao`)**: A high-performance command-line tool for batch or single-image quality analysis. It uses a zero-copy memory approach for maximum efficiency.
3.  **REST API (`OFIQ.RestApi`)**: An ASP.NET Core service providing three levels of analysis:
    *   **Method A (Scalar)**: Returns the overall quality score.
    *   **Method B (Vector)**: Returns detailed quality metrics and face bounding boxes.
    *   **Method C (Preprocessing)**: Returns metrics, bounding boxes, and 98 facial landmarks.

---

## Prerequisites

To build and run this project, you need:

- **Operating System**: Linux (x64) or WSL2.
- **Compiler**: GCC (g++) for the native bridge.
- **SDK**: .NET 10.0 SDK.
- **Docker**: (Optional) For containerized deployment.
- **OFIQ Assets**: You must provide the required `.onnx` models and `.jaxn` configuration files in the `OFIQConsoleApp/models` directory.

---

## Compilation and Setup

All components can be compiled and deployed using the unified setup script:

```bash
chmod +x setup.sh
./setup.sh
```

**What the script does:**
1. Cleans up previous build artifacts.
2. Compiles the C++ Bridge into `libofiq_bridge.so`.
3. Publishes the Console App and REST API for Linux-x64.
4. Distributes native libraries and configuration files to the `dist/` directory.
5. Automatically builds a Docker image (`ofiq-rest-api:latest`) and pushes it to Docker Hub (if Docker is available).

---

## Running the Applications

After running `setup.sh`, all artifacts are located in the `dist/` folder.

### 1. Console Application
```bash
cd dist/console
./ikao <path_to_image.jpg>
```

### 2. REST API (Local)
```bash
cd dist/api
./OFIQ.RestApi
```
The API will be available at `http://localhost:5000` (or the configured port). Documentation is available via Swagger at `/swagger`.

### 3. REST API (Docker)
```bash
docker run -p 8080:8080 tdav/ofiq-api:latest

docker run -d --restart=always --name icao-image -p 8080:8080 tdav/ofiq-api:latest
```

---

## Architecture and Performance

- **Zero-Copy Memory**: The project uses direct memory access to pass pixel data from ImageSharp to the native library, eliminating redundant allocations and CPU-heavy memory copies.
- **Resilience**: The REST API includes global exception handling and automatic fallback mechanisms for image memory access to ensure 100% stability.

---

