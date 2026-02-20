# Implementation Plan: Unified Setup and Deployment Script

## Phase 1: Script Logic Update [checkpoint: e1d58b1]
- [x] Task: Create new directory structure variables in setup.sh (DIST_DIR, CONFIG_DIR, etc.).
- [x] Task: Add cleanup logic to remove the existing dist/ directory.
- [x] Task: Update C++ bridge compilation part to output directly to the root.
- [x] Task: Conductor - User Manual Verification 'Phase 1: Script Logic' (Protocol in workflow.md)

## Phase 2: Build and Deploy Components [checkpoint: fbd3a31]
- [x] Task: Update dotnet publish command for Console App to target dist/console.
- [x] Task: Add dotnet publish command for REST API project to target dist/api.
- [x] Task: Implement library distribution logic (copying .so files to both console and api subfolders).
- [x] Task: Implement configuration and models deployment to dist/config.
- [x] Task: Conductor - User Manual Verification 'Phase 2: Build and Deploy' (Protocol in workflow.md)

## Phase 3: Final Polishing and Verification [checkpoint: f6ea244]
- [x] Task: Update summary messages at the end of the script with new run instructions.
- [x] Task: Add executable permissions check/setting within the script for собранных binaries.
- [x] Task: Verify overall script flow by running a simulated build process.
- [x] Task: Conductor - User Manual Verification 'Phase 3: Final Verification' (Protocol in workflow.md)
