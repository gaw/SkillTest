using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("VoxelEngine/Map")]
public class Map : MonoBehaviour {
	
	[SerializeField] private BlockSet blockSet;
	private List3D<Chunk> chunks = new List3D<Chunk>();
	private SunLightMap sunLightmap = new SunLightMap();
	private LightMap lightmap = new LightMap();
	
	
	public void SetBlockAndRecompute(BlockData block, Vector3i pos) {
		SetBlock( block, pos );
		
		Vector3i chunkPos = Chunk.ToChunkPosition(pos);
		Vector3i localPos = Chunk.ToLocalPosition(pos);
		
		SetDirty( chunkPos );
		
		if(localPos.x == 0) SetDirty( chunkPos - Vector3i.right );
		if(localPos.y == 0) SetDirty( chunkPos - Vector3i.up );
		if(localPos.z == 0) SetDirty( chunkPos - Vector3i.forward );
		
		if(localPos.x == Chunk.SIZE_X-1) SetDirty( chunkPos + Vector3i.right );
		if(localPos.y == Chunk.SIZE_Y-1) SetDirty( chunkPos + Vector3i.up );
		if(localPos.z == Chunk.SIZE_Z-1) SetDirty( chunkPos + Vector3i.forward );
		
		SunLightComputer.RecomputeLightAtPosition(this, pos);
		LightComputer.RecomputeLightAtPosition(this, pos);
	}
	
	private void SetDirty(Vector3i chunkPos) {
		Chunk chunk = GetChunk( chunkPos );
		if(chunk != null) chunk.GetChunkRendererInstance().SetDirty();
	}
	
	public void SetBlock(Block block, Vector3i pos) {
		SetBlock(new BlockData(block), pos);
	}
	public void SetBlock(Block block, int x, int y, int z) {
		SetBlock(new BlockData(block), x, y, z);
	}
	
	public void SetBlock(BlockData block, Vector3i pos) {
		SetBlock(block, pos.x, pos.y, pos.z);
	}
	public void SetBlock(BlockData block, int x, int y, int z) {
		Chunk chunk = GetChunkInstance( Chunk.ToChunkPosition(x, y, z) );
		if(chunk != null) chunk.SetBlock( block, Chunk.ToLocalPosition(x, y, z) );
	}
	
	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}
	public BlockData GetBlock(int x, int y, int z) {
		Chunk chunk = GetChunk( Chunk.ToChunkPosition(x, y, z) );
		if(chunk == null) return default(BlockData);
		return chunk.GetBlock( Chunk.ToLocalPosition(x, y, z) );
	}
	
	public int GetMaxY(int x, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, 0, z);
		chunkPos.y = chunks.GetMax().y;
		Vector3i localPos = Chunk.ToLocalPosition(x, 0, z);
		
		for(;chunkPos.y >= 0; chunkPos.y--) {
			localPos.y = Chunk.SIZE_Y-1;
			for(;localPos.y >= 0; localPos.y--) {
				Chunk chunk = chunks.SafeGet(chunkPos);
				if(chunk == null) break;
				BlockData block = chunk.GetBlock(localPos);
				if(!block.IsEmpty()) return Chunk.ToWorldPosition(chunkPos, localPos).y;
			}
		}
		
		return 0;
	}
	
	private Chunk GetChunkInstance(Vector3i chunkPos) {
		if(chunkPos.y < 0) return null;
		Chunk chunk = GetChunk(chunkPos);
		if(chunk == null) {
			chunk = new Chunk(this, chunkPos);
			chunks.AddOrReplace(chunk, chunkPos);
		}
		return chunk;
	}
	public Chunk GetChunk(Vector3i chunkPos) {
		return chunks.SafeGet(chunkPos);
	}
	
	public List3D<Chunk> GetChunks() {
		return chunks;
	}
	
	public SunLightMap GetSunLightmap() {
		return sunLightmap;
	}
	
	public LightMap GetLightmap() {
		return lightmap;
	}
	
	public void SetBlockSet(BlockSet blockSet) {
		this.blockSet = blockSet;
	}
	public BlockSet GetBlockSet() {
		return blockSet;
	}
	
}