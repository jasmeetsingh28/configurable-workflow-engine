# Configurable Workflow Engine

A minimal backend service for managing configurable state machine workflows built with .NET 8 and ASP.NET Core.

---

## 🔧 Quick Start

### Prerequisites

- .NET 8 SDK  
- Any IDE (Visual Studio, VS Code, etc.)

### Running the Application

1. Clone this repository  
2. Navigate to the project directory  
3. Run the application:

```bash
dotnet run
```

4. Open your browser to `https://localhost:5194/swagger` (or the port shown in console) to access the API documentation

---

## 🛠 Build Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Clean build artifacts
dotnet clean
```

---

## 🌐 Environment Notes

- **Development Environment:** The application runs in development mode by default  
- **Port:** Application typically runs on port 5194 (may vary)  
- **HTTPS:** The application supports HTTPS redirection  
- **Swagger UI:** Available at `/swagger` endpoint for API testing

---

## 📡 API Endpoints

### Workflow Definitions

- `POST /api/workflow/definitions` - Create a new workflow definition  
- `GET /api/workflow/definitions` - Get all workflow definitions  
- `GET /api/workflow/definitions/{id}` - Get a specific workflow definition

### Workflow Instances

- `POST /api/workflow/definitions/{definitionId}/instances` - Start a new workflow instance  
- `GET /api/workflow/instances` - Get all workflow instances  
- `GET /api/workflow/instances/{id}` - Get a specific workflow instance  
- `POST /api/workflow/instances/{instanceId}/execute` - Execute an action on a workflow instance

---

## 💡 Example Usage

### 1. Create a Document Approval Workflow

```http
POST /api/workflow/definitions
```

```json
{
  "name": "Document Approval Process",
  "states": [
    {
      "id": "draft",
      "name": "Draft",
      "isInitial": true,
      "isFinal": false,
      "enabled": true,
      "description": "Document is being created"
    },
    {
      "id": "review",
      "name": "Under Review",
      "isInitial": false,
      "isFinal": false,
      "enabled": true,
      "description": "Document is being reviewed"
    },
    {
      "id": "approved",
      "name": "Approved",
      "isInitial": false,
      "isFinal": true,
      "enabled": true,
      "description": "Document approved"
    }
  ],
  "actions": [
    {
      "id": "submit",
      "name": "Submit for Review",
      "enabled": true,
      "fromStates": ["draft"],
      "toState": "review"
    },
    {
      "id": "approve",
      "name": "Approve",
      "enabled": true,
      "fromStates": ["review"],
      "toState": "approved"
    }
  ]
}
```

### 2. Start a Workflow Instance

```http
POST /api/workflow/definitions/{definitionId}/instances
```

### 3. Execute Actions

```http
POST /api/workflow/instances/{instanceId}/execute
```

```json
{
  "actionId": "submit"
}
```

---

## 📁 Project Structure

```
WorkflowEngine/
├── Models/
│   ├── State.cs                    # State model with id, name, flags
│   ├── WorkflowAction.cs          # Action model with transitions
│   ├── WorkflowDefinition.cs      # Collection of states and actions
│   ├── WorkflowInstance.cs        # Runtime instance with history
│   └── DTOs/
│       ├── CreateWorkflowDefinitionRequest.cs
│       └── ExecuteActionRequest.cs
├── Services/
│   ├── IWorkflowService.cs        # Service interface
│   └── WorkflowService.cs         # Business logic implementation
├── Controllers/
│   └── WorkflowController.cs      # REST API endpoints
├── Program.cs                     # Application configuration
└── README.md
```

---

## 📐 Design Decisions

### Architecture

- **Layered Architecture:** Controllers → Services → Models for clear separation of concerns  
- **Dependency Injection:** Using built-in .NET DI container for loose coupling  
- **RESTful API:** Standard HTTP methods and status codes for intuitive usage

### Data Storage

- **In-Memory Storage:** Using `ConcurrentDictionary<string, T>` for thread-safe operations  
- **No Database:** Keeping it simple as per requirements  
- **Thread-Safe:** Safe for concurrent access in multi-threaded environments

### Validation

- **Definition Validation:** Ensures workflows have exactly one initial state, no duplicate IDs  
- **Action Execution Validation:** Prevents invalid state transitions and actions on final states  
- **Error Handling:** Clear error messages with appropriate HTTP status codes

---

## 🔄 Core Concepts

### State

- `id` - Unique identifier within the workflow definition  
- `name` - Human-readable name  
- `isInitial` - Marks the starting state (exactly one per definition)  
- `isFinal` - Marks terminal states (no outgoing actions allowed)  
- `enabled` - Controls state availability  
- `description` - Optional state description

### Action (Transition)

- `id` - Unique identifier within the workflow definition  
- `name` - Human-readable name  
- `enabled` - Controls action availability  
- `fromStates` - Collection of valid source states  
- `toState` - Single target state

### Workflow Definition

- Collection of states and actions  
- Must contain exactly one initial state  
- Validates state and action references

### Workflow Instance

- Runtime execution of a workflow definition  
- Tracks current state and execution history  
- Maintains audit trail of all state transitions

---

## ⚙️ Assumptions

1. **Persistence:** Data is stored in memory and will be lost on application restart  
2. **Concurrency:** Multiple clients can interact with the system simultaneously  
3. **State Machine Rules:** Standard state machine behavior (no parallel states, conditional transitions)  
4. **Security:** No authentication/authorization implemented (out of scope for this exercise)  
5. **Error Handling:** Invalid operations return appropriate HTTP status codes with descriptive messages


---

## ⚠️ Known Limitations

- **No Persistent Storage:** Data is lost when the application restarts  
- **No User Authentication:** No access control or user management  
- **No Advanced Features:** No parallel execution, conditional logic, or sub-workflows  
- **No Versioning:** No workflow definition versioning or migration support  
- **No Notifications:** No email alerts or external system integration  
- **Memory Usage:** In-memory storage may not be suitable for large-scale production use

---

## 🧪 Shortcuts Taken

1. **In-Memory Storage:** Used concurrent collections instead of database for simplicity  
2. **Minimal Validation:** Basic validation rules implemented, could be expanded  
3. **No Unit Tests:** Focused on functionality over test coverage within time constraints  
4. **Simple Error Messages:** Basic error handling without detailed logging  
5. **No Configuration:** Hard-coded settings instead of configuration files

---

## 💻 Technical Implementation Notes

- Built with .NET 8 and ASP.NET Core  
- Uses minimal API dependencies as requested  
- Implements proper error handling and validation  
- Thread-safe in-memory storage using concurrent collections  
- Follows REST conventions for API design  
- Includes comprehensive Swagger documentation

---

## ⏱ Time Investment

This implementation was completed within the suggested 2-hour timeframe, focusing on:

- Core functionality over advanced features  
- Clean, maintainable code structure  
- Proper validation and error handling  
- Comprehensive testing capabilities

---

## ✅ Testing Your Implementation

### Complete Test Flow

1. **Create Definition:** Use the sample JSON above  
2. **Verify Definition:** `GET /api/workflow/definitions/{id}`  
3. **Start Instance:** `POST /api/workflow/definitions/{id}/instances`  
4. **Execute Action:** `POST /api/workflow/instances/{id}/execute` with `{"actionId": "submit"}`  
5. **Check Progress:** `GET /api/workflow/instances/{id}` to see current state and history

### Expected Behavior

- Instance starts in `"draft"` state  
- `"submit"` action moves to `"review"` state  
- `"approve"` action moves to `"approved"` state (final)  
- History tracks all transitions with timestamps

---

> The design prioritizes simplicity and clarity while maintaining extensibility for future enhancements.