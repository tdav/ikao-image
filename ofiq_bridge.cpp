#include "ofiq_lib.h"
#include <memory>
#include <string>
#include <vector>
#include <cstring>

extern "C" {

    typedef struct {
        int code;
        char* info;
    } BridgeReturnStatus;

    void free_status(BridgeReturnStatus status) {
        if (status.info) {
            free(status.info);
        }
    }

    typedef void* OFIQInterface;

    OFIQInterface ofiq_get_implementation() {
        auto impl = OFIQ::Interface::getImplementation();
        // We need to keep the shared_ptr alive. 
        // We'll store it in a heap-allocated shared_ptr.
        return new std::shared_ptr<OFIQ::Interface>(impl);
    }

    void ofiq_destroy_implementation(OFIQInterface handle) {
        delete static_cast<std::shared_ptr<OFIQ::Interface>*>(handle);
    }

    BridgeReturnStatus ofiq_initialize(OFIQInterface handle, const char* configDir, const char* configFileName) {
        auto impl = *static_cast<std::shared_ptr<OFIQ::Interface>*>(handle);
        auto status = impl->initialize(configDir, configFileName);
        
        BridgeReturnStatus bridgeStatus;
        bridgeStatus.code = static_cast<int>(status.code);
        bridgeStatus.info = strdup(status.info.c_str());
        return bridgeStatus;
    }

    BridgeReturnStatus ofiq_get_version(OFIQInterface handle, int* major, int* minor, int* patch) {
        auto impl = *static_cast<std::shared_ptr<OFIQ::Interface>*>(handle);
        impl->getVersion(*major, *minor, *patch);
        
        BridgeReturnStatus bridgeStatus;
        bridgeStatus.code = 0; // Success
        bridgeStatus.info = strdup("");
        return bridgeStatus;
    }

    typedef struct {
        int measure;
        double rawScore;
        double scalar;
        int code;
    } BridgeAssessment;

    typedef struct {
        int16_t x;
        int16_t y;
        int16_t width;
        int16_t height;
    } BridgeBoundingBox;

    BridgeReturnStatus ofiq_vector_quality(
        OFIQInterface handle, 
        uint16_t width, uint16_t height, uint8_t depth, uint8_t* data, 
        BridgeAssessment* results, int* count,
        BridgeBoundingBox* bbox) 
    {
        auto impl = *static_cast<std::shared_ptr<OFIQ::Interface>*>(handle);
        
        OFIQ::Image image;
        image.width = width;
        image.height = height;
        image.depth = depth;
        
        // Use custom deleter that does nothing since we don't own the memory
        // and vectorQuality is a synchronous call.
        image.data.reset(data, [](uint8_t*){});

        OFIQ::FaceImageQualityAssessment assessments;
        auto status = impl->vectorQuality(image, assessments);

        if (status.code == OFIQ::ReturnCode::Success) {
            // Fill Bounding Box
            bbox->x = assessments.boundingBox.xleft;
            bbox->y = assessments.boundingBox.ytop;
            bbox->width = assessments.boundingBox.width;
            bbox->height = assessments.boundingBox.height;

            // Fill Assessments
            int i = 0;
            for (const auto& [measure, result] : assessments.qAssessments) {
                results[i].measure = static_cast<int>(measure);
                results[i].rawScore = result.rawScore;
                results[i].scalar = result.scalar;
                results[i].code = static_cast<int>(result.code);
                i++;
            }
            *count = i;
        }

        BridgeReturnStatus bridgeStatus;
        bridgeStatus.code = static_cast<int>(status.code);
        bridgeStatus.info = strdup(status.info.c_str());
        return bridgeStatus;
    }
}
