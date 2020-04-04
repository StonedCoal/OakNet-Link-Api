package de.oaknetwork.oaknetlink.api.network.udp.packets;

import java.util.HashMap;
import java.util.Map;

import de.oaknetwork.oaknetlink.api.game.Game;
import de.oaknetwork.oaknetlink.api.game.GameHelper;
import de.oaknetwork.oaknetlink.api.log.Logger;
import de.oaknetwork.oaknetlink.api.network.udp.UDPEndpoint;

/**
 * Using this packet, the client asks for a connection to the given Game.
 * 
 * It provides a gameName and optional a password.
 * 
 * @author Fabian Fila
 */
public class UDPRequestJoinPacket extends UDPPacket{

	@Override
	public Map<String, Class<?>> expectedTypes() {
		Map<String, Class<?>> expectedTypes = new HashMap<String, Class<?>>();
		expectedTypes.put("gameName", String.class);
		expectedTypes.put("password", String.class);
		return expectedTypes;
	}

	@Override
	protected void processPacket(Map<String, Object> data, UDPEndpoint sender) {
		String gameName = (String) data.get("gameName");
		Game game = GameHelper.gameByName(gameName);
		if(game == null) {
			UDPDeclinedJoinPacket.sendPacket(sender, "Game not found!");
			return;
		}
		if(!game.password.equals("")){
			String password = (String) data.get("Password");
			if(!game.password.equals(password)) {
				UDPDeclinedJoinPacket.sendPacket(sender, "Wrong password!");
				return;
			}
		}
		Logger.logInfo(sender.userName + " connects to " + game.owner.name, UDPRequestJoinPacket.class);
		// TODO send establishTunnelPacket to the two peers
	}
	
	public static void sendPacket(UDPEndpoint receiver, String gameName, String password) {
		Map<String, Object> data = new HashMap<String, Object>();
		data.put("gameName", gameName);
		data.put("password", password);
		sendPacket(UDPRequestJoinPacket.class, receiver, data);
	}
}