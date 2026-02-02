# TODO: Implement Bidirectional Sync between PostgreSQL and MongoDB

## Tasks
- [x] Update IncidenciasController.cs to populate Datos field in logs with full incidence data
- [x] Create SyncService.cs to handle synchronization from MongoDB logs to PostgreSQL
- [x] Add POST /api/incidencias/sync endpoint in IncidenciasController.cs
- [x] Register SyncService in Program.cs for dependency injection
- [ ] Test the sync functionality manually
- [ ] Add direct CRUD operations in MongoDB (optional)
- [ ] Document transformation rules and conflict handling
- [ ] (Optional) Add automatic synchronization

## Progress
- [x] Plan approved by user
- [x] All code changes implemented
- [x] Build successful with only warnings (nullable properties)
- [ ] Additional features requested by user
