# Phase-Track Parallel Development Methodology

## Overview

The **Phase-Track Methodology** is a parallel development system designed to accelerate AI-assisted software development while maintaining code quality and avoiding merge conflicts. This approach enables multiple developers or AI agents to work simultaneously on different aspects of a project without stepping on each other's toes.

Originally developed for complex software systems, this methodology is particularly effective for:
- **Python backend APIs** (REST, GraphQL, microservices)
- **Data scraping systems** (web scrapers, API integrators, ETL pipelines)
- **Automation workflows** (task schedulers, data processors, bot systems)
- **Any modular software architecture** requiring parallel development

---

## The Phase-Track-Agent Model

```
┌─────────────────────────────────────────────────────────────┐
│                     PHASE DOCUMENT                          │
│   (High-level goals, architecture diagrams, success criteria)│
└─────────────────────────┬───────────────────────────────────┘
                          │
          ┌───────────────┼───────────────┐
          │               │               │
          ▼               ▼               ▼
    ┌──────────┐    ┌──────────┐    ┌──────────┐
    │ Track A  │    │ Track B  │    │ Track C  │    ...
    │ Agent 1  │    │ Agent 2  │    │ Agent 3  │
    └──────────┘    └──────────┘    └──────────┘
          │               │               │
          └───────────────┼───────────────┘
                          │
                          ▼
                    ┌──────────┐
                    │ MERGE &  │
                    │ INTEGRATE│
                    └──────────┘
```

### Three Core Concepts

1. **Phase**: A major development milestone (e.g., "MVP API", "Scraper Enhancement", "Performance Optimization")
2. **Track**: An independent workstream focusing on one domain (e.g., authentication, database, scrapers)
3. **Agent**: A developer or AI assistant assigned to implement a specific track

---

## Step 1: Create Phase Document

A **Phase** represents a major milestone in your project's development lifecycle.

### Phase Document Structure

```markdown
# Phase N: [Milestone Name]

## Overview
- What this phase delivers
- Business/technical goals
- Expected timeline

## Architecture Diagram
- System component relationships
- Data flow between services
- API contracts and event flows

## Track A: [Domain Name]
### A1: Specific task
### A2: Specific task
...

## Track B: [Domain Name]
### B1: Specific task
...

## Implementation Order
| Step | Track | Deliverable | Dependencies |
|------|-------|-------------|--------------|
| 1 | B | Database models | None |
| 2 | A | API endpoints | Track B complete |
| 3 | C | Background tasks | Track A, B complete |

## Success Criteria
- [ ] All API endpoints return correct responses
- [ ] Database migrations run without errors
- [ ] Integration tests pass
- [ ] Performance benchmarks met
```

### Key Principles for Phase Documents

1. **Self-contained** - Each phase document has all context needed
2. **Dependency-aware** - Implementation order considers dependencies
3. **Testable** - Success criteria are concrete and verifiable
4. **Architecture-first** - Diagrams and contracts before code

### Example Phases

**Phase 1: MVP Foundation**
- Basic API structure
- Database schema
- Authentication system
- Health check endpoints

**Phase 2: Core Features**
- Business logic implementation
- External API integrations
- Data validation layers
- Error handling

**Phase 3: Enhancement & Scale**
- Caching layer
- Background job processing
- Rate limiting
- Performance optimization

---

## Step 2: Design Independent Tracks

**Tracks** are parallel workstreams that can be developed independently with minimal merge conflicts.

### Track Design Rules

| Rule | Why |
|------|-----|
| **One module/domain per track** | Avoids file conflicts |
| **No cross-track dependencies within phase** | Enables parallel work |
| **Clear interfaces** | Tracks communicate via contracts, not direct coupling |
| **Numbered sub-tasks (A1, A2...)** | Preserves implementation order within track |

### Example Track Breakdown: Python API Project

| Track | Domain | Files Modified | Description |
|-------|--------|----------------|-------------|
| A | API Endpoints | `app/routes/*.py`, `app/controllers/*.py` | REST endpoints, request/response handling |
| B | Database Layer | `app/models/*.py`, `migrations/*.py` | SQLAlchemy models, Alembic migrations |
| C | Authentication | `app/auth/*.py`, `app/middleware/auth.py` | JWT, OAuth, session management |
| D | External APIs | `app/integrations/*.py`, `app/clients/*.py` | Third-party API clients |
| E | Background Tasks | `app/tasks/*.py`, `celery_config.py` | Celery/RQ workers, scheduled jobs |
| F | Data Processing | `app/processors/*.py`, `app/transformers/*.py` | ETL logic, data transformation |
| G | Logging & Monitoring | `app/logging/*.py`, `app/metrics/*.py` | Structured logging, metrics, health checks |

### Example Track Breakdown: Web Scraping System

| Track | Domain | Files Modified | Description |
|-------|--------|----------------|-------------|
| A | Scraper Core | `scrapers/base.py`, `scrapers/parsers/*.py` | Base scraper classes, HTML/JSON parsers |
| B | Site Adapters | `scrapers/sites/*.py` | Site-specific scraper implementations |
| C | Data Storage | `storage/database.py`, `storage/models.py` | Persistence layer, database schemas |
| D | Queue System | `queue/manager.py`, `queue/workers.py` | Job queue, worker processes |
| E | Rate Limiting | `ratelimit/*.py`, `proxy/manager.py` | Request throttling, proxy rotation |
| F | Data Validation | `validation/*.py`, `schemas/*.py` | Schema validation, data cleaning |
| G | Monitoring | `monitoring/*.py`, `alerts/*.py` | Error tracking, success metrics, alerts |

### Example Track Breakdown: Automation System

| Track | Domain | Files Modified | Description |
|-------|--------|----------------|-------------|
| A | Workflow Engine | `workflows/engine.py`, `workflows/base.py` | Workflow execution, state management |
| B | Action Handlers | `actions/*.py` | Individual automation actions |
| C | Trigger System | `triggers/*.py`, `schedulers/*.py` | Event triggers, cron scheduling |
| D | Data Connectors | `connectors/*.py` | External system integrations |
| E | State Management | `state/*.py`, `persistence/*.py` | Workflow state tracking |
| F | Error Recovery | `recovery/*.py`, `retry/*.py` | Failure handling, retry logic |
| G | Dashboard API | `api/*.py`, `api/endpoints/*.py` | Monitoring and control API |

### Avoiding Conflicts

- **Tracks should NOT modify the same files**
- If a shared file must be touched (e.g., `config.py`, `main.py`), designate ONE track as owner
- Use interfaces, events, or message queues for cross-track communication
- Integration happens AFTER all tracks are complete

---

## Step 3: Cross-Track Communication Patterns

Tracks must communicate without creating tight coupling. Use these patterns:

### 1. Shared Interfaces / Protocols (Python)

```python
# shared/interfaces.py - Track-agnostic
from abc import ABC, abstractmethod
from typing import Dict, Any

class DataProcessor(ABC):
    @abstractmethod
    def process(self, data: Dict[str, Any]) -> Dict[str, Any]:
        pass

# Track A implements
class APIDataProcessor(DataProcessor):
    def process(self, data):
        return {"api_processed": data}

# Track C uses interface
def handle_data(processor: DataProcessor, data):
    return processor.process(data)
```

### 2. Message Queue / Event Bus

```python
# Track A - Publishes events
from app.events import event_bus

def create_user(user_data):
    user = User.create(**user_data)
    event_bus.publish('user.created', {
        'user_id': user.id,
        'email': user.email
    })
    return user

# Track E - Subscribes to events
from app.events import event_bus

@event_bus.subscribe('user.created')
def send_welcome_email(event_data):
    email_service.send_welcome(event_data['email'])
```

### 3. Configuration-Based Coupling

```yaml
# config/services.yaml - Neutral ground
api:
  endpoints:
    - name: user_service
      url: http://localhost:8000/users
      
database:
  models:
    - User
    - Product
    
tasks:
  enabled:
    - email_sender
    - data_sync
```

### 4. Database as Integration Point

```python
# Track B creates table
class JobQueue(Base):
    __tablename__ = 'job_queue'
    id = Column(Integer, primary_key=True)
    status = Column(String)
    payload = Column(JSON)

# Track A adds jobs
def enqueue_job(data):
    job = JobQueue(status='pending', payload=data)
    db.session.add(job)
    
# Track E processes jobs
def process_jobs():
    jobs = JobQueue.query.filter_by(status='pending').all()
    for job in jobs:
        handle_job(job.payload)
```

### 5. API Contracts

```python
# shared/contracts.py - Track-agnostic
from pydantic import BaseModel

class UserRequest(BaseModel):
    email: str
    name: str

class UserResponse(BaseModel):
    id: int
    email: str
    name: str
    created_at: str

# Track A uses contract
def create_user_endpoint(request: UserRequest) -> UserResponse:
    pass
```

---

## Step 4: Assign Tracks to Agents

Each **Track** is assigned to a separate AI agent or developer.

### Agent Assignment Protocol

1. **Open new agent session** (new chat window or branch)
2. **Provide context:**
   - Full phase document
   - Specific track assignment (e.g., "Implement Track A only")
   - Existing codebase patterns and style guide
3. **Agent works independently** on assigned track
4. **Agent commits** when track complete

### Agent Instructions Template

```markdown
# Track [X] Implementation: [Domain Name]

## Your Scope
You are implementing Track [X] of Phase [N].

### Files to Create
- `path/to/new_module.py` - Description
- `tests/test_new_module.py` - Unit tests

### Files to Modify
- `path/to/existing.py` - Add method X, update class Y

### Files NOT to Touch
- `path/to/track_b_file.py` - Owned by Track B
- `main.py` - Integration file, modified during merge phase

## Integration Points

### Events You Subscribe To
- `user.created` - Handle new user registration
- `data.imported` - Process imported data

### Events You Publish
- `order.completed` - When order processing finishes
- `error.critical` - On critical errors

### Interfaces You Implement
```python
from shared.interfaces import DataProcessor

class YourProcessor(DataProcessor):
    def process(self, data): ...
```

### APIs You Consume
- `GET /api/users/{id}` - User details
- `POST /api/notifications` - Send notification

## Dependencies
- Track B must complete database models first
- Track C provides authentication middleware

## Success Criteria
- [ ] All endpoints return 200 for valid requests
- [ ] Unit tests achieve >80% coverage
- [ ] Integration tests pass
- [ ] No linter errors
- [ ] Documentation updated

## Test Steps
1. Start local server: `python -m app.main`
2. Run tests: `pytest tests/track_x/`
3. Manual test: `curl http://localhost:8000/your-endpoint`
4. Verify logs show expected output
```

---

## Step 5: Implementation Guidelines

### DO's ✅

1. **Read track documentation first** - Understand scope before coding
2. **Stay within track boundaries** - Only modify files assigned to your track
3. **Use defined interfaces** - Don't create direct dependencies on other tracks
4. **Write tests** - Unit and integration tests for your track
5. **Document assumptions** - Note any assumptions about other tracks' behavior
6. **Commit frequently** - Small, atomic commits within your track
7. **Handle errors gracefully** - Don't assume other tracks will succeed

### DON'Ts ❌

1. **Don't modify files outside your track** - Unless explicitly permitted
2. **Don't add direct imports from other tracks** - Use interfaces/events
3. **Don't skip the planning phase** - Always review track document first
4. **Don't assume integration will "just work"** - Plan for integration issues
5. **Don't leave TODOs that depend on other tracks** - Resolve within track or use interfaces
6. **Don't change shared contracts without coordination** - Discuss breaking changes

---

## Step 6: Merge & Integration

After all tracks complete, the integration phase begins.

### Integration Checklist

1. **Merge all track branches**
   - Resolve any file conflicts (should be minimal with good track design)
   - Review all changes together

2. **Wire up cross-track communication**
   - Connect event publishers to subscribers
   - Configure service discovery
   - Set up message queue routing

3. **Run integration tests**
   - Test cross-track workflows
   - Verify event flows work end-to-end
   - Load test critical paths

4. **Verify success criteria**
   - Check phase document success criteria
   - Run smoke tests on all features

5. **Fix integration bugs**
   - Debug cross-track issues
   - Update interfaces if needed

### Common Integration Issues

| Issue | Cause | Fix |
|-------|-------|-----|
| Import errors | Missing dependencies between tracks | Add to requirements.txt, install packages |
| Events not received | Subscriber not registered | Check event bus initialization |
| Database errors | Migration order wrong | Reorder migrations, add dependencies |
| API errors | Contract mismatch | Update Pydantic models, sync contracts |
| Null/None errors | Initialization order | Use dependency injection, fix startup order |
| Config missing | Track expects config not set | Update config files, add defaults |
| Connection refused | Service not started | Check Docker Compose, service health |

### Integration Testing Script Example

```python
# tests/integration/test_full_workflow.py
import pytest
from app import create_app, db
from app.models import User, Order

def test_complete_order_workflow():
    """Test Track A (API) -> Track B (DB) -> Track E (Tasks)"""
    
    # Track A: Create user via API
    response = client.post('/api/users', json={
        'email': 'test@example.com',
        'name': 'Test User'
    })
    assert response.status_code == 201
    user_id = response.json['id']
    
    # Track B: Verify database record
    user = User.query.get(user_id)
    assert user is not None
    assert user.email == 'test@example.com'
    
    # Track A: Create order
    response = client.post('/api/orders', json={
        'user_id': user_id,
        'items': [{'sku': 'ABC', 'qty': 2}]
    })
    assert response.status_code == 201
    
    # Track E: Verify background task triggered
    from app.tasks import email_queue
    assert email_queue.has_task('send_order_confirmation')
```

---

## Benefits of This Approach

### 1. Parallelization
- N agents working simultaneously = N× throughput
- No waiting for dependencies between agents
- Faster time to completion

### 2. Reduced Context Switching
- Each agent focuses on ONE domain
- Deep understanding of assigned track
- Less cognitive load

### 3. Minimized Merge Conflicts
- Tracks own specific files/folders
- Git merges are clean
- Less time resolving conflicts

### 4. Clear Accountability
- Each track has defined deliverables
- Easy to identify which track has issues
- Better project tracking

### 5. Modularity
- Forces good separation of concerns
- Promotes loose coupling
- Results in maintainable architecture

### 6. Scalability
- Add more tracks = add more agents
- Works for solo dev (sequential) or team (parallel)
- Scales with project complexity

---

## When NOT to Use This Approach

| Scenario | Why Not | Alternative |
|----------|---------|-------------|
| **Tightly coupled refactor** | Everything touches everything | Single agent, sequential refactor |
| **Exploration/prototyping** | Scope unclear, needs experimentation | Single agent, iterative prototyping |
| **Bug fixing** | Often cross-cutting concerns | Single agent with full context |
| **Small features** | Overhead not worth it | Just implement directly |
| **Urgent hotfixes** | No time for coordination | Single agent, immediate fix |
| **Learning new tech** | Need to understand holistically | Single agent, full-stack learning |

---

## Best Practices from Production Use

### Architecture & Planning

1. **Define MVP first** - Don't start without clear scope and milestones
   - Scope creep destroys projects
   - Ship vertical slices: MVP → Enhancement → Polish

2. **Design before coding** - Keep a living design document
   - System diagrams (components, data flow)
   - API contracts and event schemas
   - Update as you iterate

3. **Don't over-engineer** - Start simple, refactor when needed
   - Add patterns (DI, events) when justified by actual need
   - Prefer simple solutions

### Code Organization

4. **Separate concerns** - Don't mix business logic, data access, and presentation
   - Use layered architecture (routes → services → repositories → models)
   - Each module has one responsibility

5. **Avoid global state** - Don't use singletons for everything
   - Use dependency injection
   - Configuration files for shared config
   - Connection pools for database/cache

6. **Use type hints** - Leverage Python typing for contracts
   ```python
   from typing import List, Optional
   
   def get_users(limit: int = 10) -> List[User]:
       pass
   ```

### Performance

7. **Avoid blocking operations** - Use async for I/O bound tasks
   ```python
   import asyncio
   import aiohttp
   
   async def fetch_data(url: str) -> dict:
       async with aiohttp.ClientSession() as session:
           async with session.get(url) as response:
               return await response.json()
   ```

8. **Use connection pooling** - Don't create new connections per request
   ```python
   from sqlalchemy import create_engine, pool
   
   engine = create_engine(
       DATABASE_URL,
       poolclass=pool.QueuePool,
       pool_size=10,
       max_overflow=20
   )
   ```

9. **Implement caching** - Cache expensive operations
   ```python
   from functools import lru_cache
   
   @lru_cache(maxsize=128)
   def get_expensive_data(key: str):
       return expensive_operation(key)
   ```

### Testing

10. **Test critical paths** - Don't skip tests
    - Unit tests for business logic
    - Integration tests for workflows
    - E2E tests for critical user journeys

11. **Use test fixtures** - Make tests reproducible
    ```python
    import pytest
    
    @pytest.fixture
    def sample_user():
        return User(email='test@example.com', name='Test')
    
    def test_user_creation(sample_user):
        assert sample_user.email == 'test@example.com'
    ```

### Monitoring & Debugging

12. **Structured logging** - Use JSON logging for production
    ```python
    import structlog
    
    logger = structlog.get_logger()
    logger.info('user.created', user_id=123, email='test@example.com')
    ```

13. **Health checks** - Implement liveness and readiness probes
    ```python
    @app.get('/health')
    def health_check():
        return {'status': 'healthy', 'database': db.is_connected()}
    ```

14. **Metrics** - Track key performance indicators
    ```python
    from prometheus_client import Counter, Histogram
    
    request_count = Counter('requests_total', 'Total requests')
    request_duration = Histogram('request_duration_seconds', 'Request duration')
    ```

---

## Templates

### Phase Document Template

```markdown
# Phase [N]: [Milestone Name]

## Overview
[Brief description of what this phase delivers]

**Timeline**: [Expected duration]
**Priority**: [High/Medium/Low]

## Goals
- Business goal 1
- Business goal 2
- Technical goal 1

## Architecture

### System Components
```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│   API Layer │─────▶│  Services   │─────▶│  Database   │
└─────────────┘      └─────────────┘      └─────────────┘
       │                    │
       │                    ▼
       │             ┌─────────────┐
       └────────────▶│ Background  │
                     │    Jobs     │
                     └─────────────┘
```

### Data Flow
1. Request arrives at API endpoint
2. Service validates and processes
3. Data persisted to database
4. Event published to queue
5. Background job processes asynchronously

## Track Breakdown

### Track A: [Domain Name]
**Owner**: [Agent/Developer name]
**Files**: `path/to/files/*`

#### Tasks
- A1: [Specific task]
- A2: [Specific task]
- A3: [Specific task]

#### Dependencies
- None (or list dependencies)

#### Success Criteria
- [ ] Criterion 1
- [ ] Criterion 2

### Track B: [Domain Name]
**Owner**: [Agent/Developer name]
**Files**: `path/to/other/files/*`

#### Tasks
- B1: [Specific task]
- B2: [Specific task]

#### Dependencies
- Track A must complete A1 first

#### Success Criteria
- [ ] Criterion 1

## Implementation Order

| Step | Track | Deliverable | Dependencies | Estimated Time |
|------|-------|-------------|--------------|----------------|
| 1 | B | Database models | None | 2 hours |
| 2 | C | Auth middleware | None | 3 hours |
| 3 | A | API endpoints | Track B, C | 4 hours |
| 4 | E | Background tasks | Track A | 3 hours |
| 5 | - | Integration | All tracks | 2 hours |

## Integration Points

### Events
- `user.created` - Published by Track A, consumed by Track E
- `order.completed` - Published by Track E, consumed by Track A

### API Contracts
```python
class UserRequest(BaseModel):
    email: str
    name: str

class UserResponse(BaseModel):
    id: int
    email: str
```

### Shared Interfaces
```python
class DataProcessor(ABC):
    @abstractmethod
    def process(self, data: Dict) -> Dict:
        pass
```

## Success Criteria

### Functional
- [ ] All API endpoints return expected responses
- [ ] Database migrations run without errors
- [ ] Background jobs execute successfully
- [ ] Error handling works for edge cases

### Non-Functional
- [ ] Response time < 200ms for 95th percentile
- [ ] Unit test coverage > 80%
- [ ] Zero security vulnerabilities
- [ ] Documentation complete

## Testing Strategy

### Unit Tests
- Each track tests its own modules
- Mock external dependencies

### Integration Tests
- Test cross-track workflows
- Use test database

### Load Tests
- Simulate expected traffic
- Identify bottlenecks

## Rollout Plan

1. Deploy to staging
2. Run smoke tests
3. Performance testing
4. Deploy to production
5. Monitor metrics

## Rollback Plan

If issues arise:
1. Revert deployment
2. Restore database backup (if needed)
3. Investigate root cause
4. Fix and redeploy
```

### Track Assignment Prompt Template

```markdown
# Track [X] Implementation: [Domain Name]

You are implementing Track [X] for Phase [N]: [Phase Name].

## Context
[Brief overview of what this track delivers and why it matters]

## Your Scope

### Files to Create
- `app/module_name/service.py` - Core service logic
- `app/module_name/models.py` - Data models
- `tests/test_module_name.py` - Test suite

### Files to Modify
- `app/main.py` - Register your routes (lines 45-50)
- `requirements.txt` - Add dependencies: `package==1.0.0`

### Files NOT to Touch
- `app/other_track/` - Owned by Track [Y]
- `config/production.yaml` - Modified during integration

## Integration Points

### Events You Subscribe To
```python
@event_bus.subscribe('event.name')
def handle_event(data: Dict):
    # Your handler
    pass
```

### Events You Publish
```python
event_bus.publish('your.event', {
    'key': 'value'
})
```

### APIs You Consume
- `GET /api/resource/{id}` - Returns Resource object
- `POST /api/action` - Triggers action

### Interfaces You Implement
```python
from shared.interfaces import ProcessorInterface

class YourProcessor(ProcessorInterface):
    def process(self, data: Dict) -> Dict:
        # Implementation
        pass
```

## Dependencies
- Track [Y] must complete first (provides database models)
- Track [Z] runs in parallel (no dependencies)

## Implementation Tasks

### [X]1: [Task Name]
**Description**: [What needs to be done]

**Files**: `path/to/file.py`

**Acceptance Criteria**:
- [ ] Functionality works as expected
- [ ] Tests pass
- [ ] No linter errors

### [X]2: [Task Name]
**Description**: [What needs to be done]

**Files**: `path/to/other/file.py`

**Acceptance Criteria**:
- [ ] Functionality works as expected
- [ ] Tests pass

## Code Style Guidelines

Follow existing patterns in the codebase:
- Use type hints for all function signatures
- Write docstrings for public functions
- Keep functions under 50 lines
- Use descriptive variable names
- Follow PEP 8 style guide

```python
def example_function(param1: str, param2: int) -> Dict[str, Any]:
    """
    Brief description of what this function does.
    
    Args:
        param1: Description of param1
        param2: Description of param2
        
    Returns:
        Dictionary containing result
    """
    result = {}
    # Implementation
    return result
```

## Testing Requirements

### Unit Tests
```python
import pytest
from app.module_name import YourClass

def test_your_function():
    instance = YourClass()
    result = instance.your_method('input')
    assert result == 'expected_output'
```

### Integration Tests
```python
def test_integration_workflow(test_client, test_db):
    response = test_client.post('/api/endpoint', json={'data': 'value'})
    assert response.status_code == 201
    assert response.json['id'] is not None
```

## Success Criteria

When you're done, verify:
- [ ] All tasks ([X]1, [X]2, ...) completed
- [ ] Unit tests pass: `pytest tests/track_x/`
- [ ] Integration tests pass
- [ ] Code coverage > 80%
- [ ] No linter errors: `flake8 app/module_name/`
- [ ] Type checking passes: `mypy app/module_name/`
- [ ] Documentation updated

## Test Steps

1. **Setup environment**
   ```bash
   python -m venv venv
   source venv/bin/activate  # or `venv\Scripts\activate` on Windows
   pip install -r requirements.txt
   ```

2. **Run unit tests**
   ```bash
   pytest tests/track_x/ -v
   ```

3. **Start local server**
   ```bash
   python -m app.main
   ```

4. **Manual testing**
   ```bash
   # Test endpoint
   curl -X POST http://localhost:8000/api/your-endpoint \
     -H "Content-Type: application/json" \
     -d '{"key": "value"}'
   ```

5. **Check logs**
   - Verify expected log messages appear
   - No error stack traces

## Questions?

If you encounter issues or need clarification:
1. Check existing code patterns in similar modules
2. Review the phase document for architecture details
3. Ask specific questions about integration points
4. Document assumptions you make
```

---

## Real-World Example: Building a Data Scraping System

### Phase 1: MVP Scraper

**Goal**: Scrape product data from 3 e-commerce sites, store in database, expose via API.

### Track Breakdown

#### Track A: Scraper Core
- `scrapers/base_scraper.py` - Abstract base scraper
- `scrapers/parser.py` - HTML parsing utilities
- `scrapers/http_client.py` - HTTP client with retries

#### Track B: Site Implementations
- `scrapers/sites/amazon.py` - Amazon scraper
- `scrapers/sites/ebay.py` - eBay scraper
- `scrapers/sites/walmart.py` - Walmart scraper

#### Track C: Database Layer
- `models/product.py` - SQLAlchemy Product model
- `repositories/product_repo.py` - CRUD operations
- `migrations/001_create_products.py` - Initial schema

#### Track D: API Layer
- `api/routes/products.py` - REST endpoints
- `api/schemas.py` - Pydantic models
- `api/main.py` - FastAPI application

#### Track E: Job Queue
- `tasks/scrape_job.py` - Celery task definition
- `tasks/scheduler.py` - Periodic job scheduling
- `celery_app.py` - Celery configuration

### Implementation Order

1. **Track C** (Database) - No dependencies
2. **Track A** (Scraper Core) - No dependencies
3. **Track B** (Site Scrapers) - Depends on A
4. **Track E** (Job Queue) - Depends on A, B, C
5. **Track D** (API) - Depends on C
6. **Integration** - Wire everything together

### Integration Points

```python
# Track B publishes events after scraping
from app.events import event_bus

class AmazonScraper(BaseScraper):
    def scrape(self, url: str):
        data = self._parse_page(url)
        event_bus.publish('product.scraped', {
            'source': 'amazon',
            'data': data
        })

# Track C subscribes to save data
@event_bus.subscribe('product.scraped')
def save_product(event_data):
    product_repo.create(event_data['data'])

# Track D exposes via API
@app.get('/api/products')
def get_products(skip: int = 0, limit: int = 10):
    return product_repo.get_all(skip=skip, limit=limit)
```

---

## Tools & Resources

### Python Project Structure
```
project/
├── app/
│   ├── __init__.py
│   ├── main.py              # Application entry point
│   ├── config.py            # Configuration management
│   ├── models/              # Database models (Track C)
│   ├── routes/              # API endpoints (Track A)
│   ├── services/            # Business logic (Track D)
│   ├── tasks/               # Background jobs (Track E)
│   ├── integrations/        # External APIs (Track F)
│   └── utils/               # Shared utilities
├── tests/
│   ├── unit/
│   ├── integration/
│   └── fixtures/
├── migrations/              # Database migrations
├── docs/                    # Phase documents
├── requirements.txt         # Dependencies
├── docker-compose.yml       # Local development
└── README.md
```

### Recommended Tools

**Framework**: FastAPI, Flask, Django
**Database**: PostgreSQL, MySQL, MongoDB
**ORM**: SQLAlchemy, Django ORM
**Migrations**: Alembic, Django migrations
**Task Queue**: Celery, RQ, Dramatiq
**Message Broker**: Redis, RabbitMQ
**HTTP Client**: httpx, aiohttp, requests
**Testing**: pytest, unittest
**Linting**: flake8, pylint, black
**Type Checking**: mypy, pyright
**Monitoring**: Prometheus, Grafana, Sentry

### Git Workflow

```bash
# Create feature branch for track
git checkout -b phase1/track-a-api-endpoints

# Work on track
git add app/routes/
git commit -m "Track A: Add user endpoints"

# Push track branch
git push origin phase1/track-a-api-endpoints

# After all tracks complete, merge to main
git checkout main
git merge phase1/track-a-api-endpoints
git merge phase1/track-b-database
git merge phase1/track-c-auth
```

---

## FAQ

### Q: How many tracks should one phase have?

**A**: Typically 3-7 tracks per phase. More than 7 becomes hard to coordinate. Fewer than 3 might not justify the overhead.

### Q: Can one person implement multiple tracks?

**A**: Yes! Sequential implementation still benefits from modular design. Work on Track A, commit, then Track B, etc.

### Q: What if tracks need to share code?

**A**: Create a shared module that both tracks can import. Define interfaces early in the phase document.

### Q: How do I handle urgent bugs during track development?

**A**: Pause track work, fix the bug in a hotfix branch, merge to main, then rebase track branches.

### Q: Should I use this for small projects?

**A**: No. This methodology shines for medium-to-large projects (10K+ lines). Small projects have too much overhead.

### Q: Can AI agents really work independently?

**A**: Yes, if given clear instructions and boundaries. The track document is crucial for AI success.

### Q: What if integration fails?

**A**: This usually means tracks weren't independent enough. Refactor to add interfaces/events between tracks.

---

## Conclusion

The Phase-Track Methodology enables parallel development by:
1. Breaking work into independent modules (tracks)
2. Defining clear boundaries and interfaces
3. Allowing multiple agents to work simultaneously
4. Minimizing merge conflicts through smart file ownership
5. Enforcing integration testing at phase boundaries

**Key Takeaway**: Invest time upfront in phase planning and track design. The payoff is faster development, cleaner code, and fewer integration headaches.

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-12 | Initial generalized methodology for Python/backend projects |

---

## License

This methodology is free to use and adapt for any software project.
