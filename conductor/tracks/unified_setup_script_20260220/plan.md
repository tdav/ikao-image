# Implementation Plan: Unified Setup and Deployment Script

## Phase 1: Script Logic Update
- [ ] Task: Create new directory structure variables in setup.sh (DIST_DIR, CONFIG_DIR, etc.).
- [ ] Task: Add cleanup logic to remove the existing dist/ directory.
- [ ] Task: Update C++ bridge compilation part to output directly to the root.
- [ ] Task: Conductor - User Manual Verification 'Phase 1: Script Logic' (Protocol in workflow.md)

## Phase 2: Build and Deploy Components
- [ ] Task: Update dotnet publish command for Console App to target dist/console.
- [ ] Task: Add dotnet publish command for REST API project to target dist/api.
- [ ] Task: Implement library distribution logic (copying .so files to both console and api subfolders).
- [ ] Task: Implement configuration and models deployment to dist/config.
- [ ] Task: Conductor - User Manual Verification 'Phase 2: Build and Deploy' (Protocol in workflow.md)

## Phase 3: Final Polishing and Verification
- [ ] Task: Update summary messages at the end of the script with new run instructions.
- [ ] Task: Add executable permissions check/setting within the script for собранных binaries.
- [ ] Task: Verify overall script flow by running a simulated build process.
- [ ] Task: Conductor - User Manual Verification 'Phase 3: Final Verification' (Protocol in workflow.md)
