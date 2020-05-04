#  Copyright (c) Microsoft Corporation. All rights reserved.
#  Licensed under the MIT License.

cmake_minimum_required (VERSION 3.8)
project (AzureSphereAuthenticationBackend_HighLevelCore C)

# Create executable
add_executable (${PROJECT_NAME} main.c rgbled_utility.c azure_iot_utilities.c epoll_timerfd_utilities.c parson.c)
target_include_directories(${PROJECT_NAME} PUBLIC ${AZURE_SPHERE_API_SET_DIR}/usr/include/azureiot)
target_compile_definitions(${PROJECT_NAME} PUBLIC AZURE_IOT_HUB_CONFIGURED)
target_link_libraries (${PROJECT_NAME} applibs pthread gcc_s c -lm -lazureiot )

# Add MakeImage post-build command
include (${AZURE_SPHERE_MAKE_IMAGE_FILE})
