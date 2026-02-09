{
  description = "ErsatzTV development environment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
        dotnet-sdk = pkgs.dotnet-sdk_10;
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = [
            dotnet-sdk
            pkgs.dotnet-ef
            pkgs.ffmpeg
          ];
          shellHook = ''
            export DOTNET_ROOT=${dotnet-sdk};
            export PATH="$PATH:$HOME/.dotnet/tools"
          '';
        };
      });
}
